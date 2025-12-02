using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySystem : System<EntitySystem>
{
    public static int DEFAULT_ID = -9999;
    
    private int entityIdCounter;
    private Dictionary<int, Entity> entityDict;
    
    protected override void OnInit()
    {
        entityDict = new Dictionary<int, Entity>();
        entityIdCounter = 0;
    }

    protected override void OnShutdown()
    {
        List<Entity> entityList = new List<Entity>(entityDict.Values);
        foreach (Entity entity in entityList)
        {
            entity.Dispose();
        }
        entityDict.Clear();
        entityDict = null;
        entityIdCounter = 0;
    }

    private int GetNewEntityId()
    {
        entityIdCounter++;
        return entityIdCounter;
    }

    public void OnEntityCreated(Entity entity)
    {
        int entityId;
        if (entity.id == DEFAULT_ID)
        {
            entityId = GetNewEntityId();
            while (entityDict.ContainsKey(entityId))
            {
                entityId = GetNewEntityId();
            }
            
            entity.id = entityId;
        }
        else
        {
            entityId = entity.id;
            if (entityDict.ContainsKey(entity.id))
            {
                MinimalEnvironment.Instance.GetSystem<LogSystem>().Error($"[EntityManager] entity id {entity.id} already exists");
                Entity oldEntity = GetEntity(entity.id);
                oldEntity.Dispose();
            }
        }
        entityDict.Add(entityId, entity);
    }

    public void OnEntityDestroyed(Entity entity)
    {
        if (entityDict.ContainsKey(entity.id))
        {
            entityDict.Remove(entity.id);
        }
        else
        {
            MinimalEnvironment.Instance.GetSystem<LogSystem>().Error($"[EntityManager] trying to remove entity {entity.id} that does not exist in dict");
        }
    }

    public Entity GetEntity(int entityId)
    {
        if (entityDict.ContainsKey(entityId))
        {
            return entityDict[entityId];
        }
        
        MinimalEnvironment.Instance.GetSystem<LogSystem>().Error($"[EntityManager] try get entity {entityId}, but does not exist in dict");
        return null;
    }
    
    // public Entity RayCastEntity(Ray ray, float maxDistance)
    // {
    //     if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, controller.clickableEntityLayerMask))
    //     {
    //         Entity entity = hit.collider.GetComponentInParent<Entity>();
    //         if (entity != null)
    //         {
    //             return entity;
    //         }
    //     }
    //     return null;
    // }
    
    public void SpawnEntity(
        Vector3 spawnPosition, 
        Quaternion spawnRotation = default,
        Action<Entity> onSpawned = null)
    {
        string prefabPath = "";
        
        MinimalEnvironment.Instance.GetSystem<ResourceSystem>().LoadAsyn<GameObject>(prefabPath, (go) =>
        {
            if (go == null)
            {
                MinimalEnvironment.Instance.GetSystem<LogSystem>().Error($"[EntityManager] failed to load prefab {prefabPath}");
                return;
            }
            
            Entity entity = go.GetComponent<Entity>();
            if (entity == null)
            {
                MinimalEnvironment.Instance.GetSystem<LogSystem>().Error($"[EntityManager] prefab {prefabPath} does not have Entity component");
                return;
            }

            entity.Init();
            
            //TODO: 使用entity的transformComponent进行初始化
            entity.transform.position = spawnPosition;
            if (spawnRotation == default)
            {
                spawnRotation = Quaternion.identity;
            }
            entity.transform.rotation = spawnRotation;
            
            OnEntityCreated(entity);
            
            entity.PostInit();
            
            onSpawned?.Invoke(entity);
        });
    }
    
    public void DisposeEntity(int entityId)
    {
        if (!entityDict.ContainsKey(entityId))
        {
            MinimalEnvironment.Instance.GetSystem<LogSystem>().Warn($"[EntityManager] trying to dispose entity {entityId}, but does not exist in dict");
            return;
        }
        Entity entity = GetEntity(entityId);
        entity.Dispose();
    }
}