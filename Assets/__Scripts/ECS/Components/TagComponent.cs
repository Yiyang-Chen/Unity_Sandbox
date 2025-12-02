using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class TagComponent : BaseComponent
{
    public TagSet tags = new TagSet();
    public Dictionary<int, TagSet> grantedTags = new Dictionary<int, TagSet>();

    public List<string> defaultTags;
    
    private int _grantedTagIdx = 0;

    [Button]
    private void LogTags()
    {
        entity.logComponent.Debug($"Tags: {tags.PrintAll()}");

        List<string> result = new List<string>();
        foreach (var kvp in grantedTags)
        {
            result.Add($"GrantID:{kvp.Key} - Tags:[{kvp.Value.PrintAll()}]");
        }

        string logMessage = string.Join("\n", result);
        
        entity.logComponent.Debug($"GrantedTags: {logMessage}");
    }

    protected override void InitComponent()
    {
        base.InitComponent();
        if (defaultTags != null)
        {
            foreach (var tagName in defaultTags)
            {
                AddTag(Tag.GetOrCreate(tagName), true);
            }
        }
    }

    protected override void DisposeComponent()
    {
        base.DisposeComponent();
    }
    
    private void OnTagChange()
    {
        entity.eventComponent.EventTrigger(EEntityEvent.OnTagChanged);
    }

    public int GrantTags(TagSet tags)
    {
        _grantedTagIdx++;
        grantedTags[_grantedTagIdx] = tags;
        OnTagChange();
        return _grantedTagIdx;
    }

    public void RevokeGrantTags(int idx)
    {
        if (grantedTags.ContainsKey(idx))
        {
            grantedTags.Remove(idx);
            OnTagChange();
        }
    }

    public bool AddTag(Tag tag, bool isInit = false)
    {
        if (tag == null) return false;

        bool ret = tags.AddTag(tag);
        if (ret && !isInit)
        {
            OnTagChange();
        }
        return ret;
    }

    public bool AddTags(TagSet tags)
    {
        if (tags.IsEmpty()) return false;

        bool ret = this.tags.AddTags(tags);
        if (ret)
        {
            OnTagChange();
        }
        return ret;
    }

    public bool RemoveTag(Tag tag, bool forceClear = false)
    {
        bool ret = tags.RemoveTag(tag, forceClear);
        if (ret)
        {
            OnTagChange();
        }
        return ret;
    }

    public bool RemoveTags(TagSet tags)
    {
        bool ret = this.tags.RemoveTags(tags);
        if (ret)
        {
            OnTagChange();
        }
        return ret;
    }

    public bool HasTag(Tag tag)
    {
        if (tags.HasTag(tag))
            return true;
        
        foreach (var gTags in grantedTags.Values)
        {
            if (gTags.HasTag(tag))
                return true;
        }
        return false;
    }

    public bool HasAllTags(TagSet tags)
    {
        foreach (var tag in tags.AllTags())
        {
            if (this.tags.HasTag(tag))
                continue;
            
            bool found = false;
            foreach (var gTags in grantedTags.Values)
            {
                if (gTags.HasTag(tag))
                {
                    found = true;
                    break;
                }
            }
            if (!found) return false;
        }
        return true;
    }

    public bool HasAnyTags(TagSet tags)
    {
        foreach (var tag in tags.AllTags())
        {
            if (this.tags.HasTag(tag))
                return true;
            
            foreach (var gTags in grantedTags.Values)
            {
                if (gTags.HasTag(tag))
                    return true;
            }
        }
        return false;
    }

    public void ClearAllTags()
    {
        tags.Clear();
        OnTagChange();
    }
}