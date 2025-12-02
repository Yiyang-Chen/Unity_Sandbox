using System;
using System.Collections.Generic;
using System.Linq;

public class TagSet
{
    //直接拥有的tag
    private Dictionary<int, int> tags = new Dictionary<int, int>();
    //因为父子关系，间接拥有的父类tag
    private Dictionary<int, int> impliedTags = new Dictionary<int, int>();

    public TagSet(params string[] tagNames)
    {
        foreach (var name in tagNames)
        {
            AddTag(Tag.GetOrCreate(name));
        }
    }

    public List<Tag> AllTags()
    {
        return tags.Keys.Select(hash => Tag.GetOrCreate(Tag.GetNameByHash(hash))).ToList();
    }

    public string PrintAll()
    {
        return string.Join(",", tags.Keys.Select(Tag.GetNameByHash));
    }
    
    public bool IsEmpty() => tags.Count == 0;
    public int Count => tags.Count;

    public bool HasTag(Tag tag)
    {
        return tags.ContainsKey(tag.Hash) || impliedTags.ContainsKey(tag.Hash);
    }

    public bool HasAnyTags(TagSet otherSet)
    {
        return otherSet.tags.Keys.Any(hash => tags.ContainsKey(hash) || impliedTags.ContainsKey(hash));
    }

    public bool HasAllTags(TagSet otherSet)
    {
        return otherSet.tags.Keys.All(hash => tags.ContainsKey(hash) || impliedTags.ContainsKey(hash));
    }

    public bool AddTag(Tag tag)
    {
        if (tags.TryGetValue(tag.Hash, out var count))
        {
            tags[tag.Hash] = count + 1;
            return false;
        }

        tags[tag.Hash] = 1;
        foreach (var parent in tag.Parents.Values)
        {
            impliedTags.TryGetValue(parent.Hash, out var parentCount);
            impliedTags[parent.Hash] = parentCount + 1;
        }
        return true;
    }

    public bool AddTags(TagSet otherSet)
    {
        if (otherSet == null) return false;
        
        bool changed = false;
        foreach (var tag in otherSet.AllTags())
        {
            changed = AddTag(tag) || changed;
        }
        return changed;
    }

    public bool RemoveTag(Tag tag, bool forceClear = false)
    {
        if (!tags.TryGetValue(tag.Hash, out var count))
            return false;

        if (count > 1 && !forceClear)
        {
            tags[tag.Hash] = count - 1;
            return false;
        }

        tags.Remove(tag.Hash);
        foreach (var parent in tag.Parents.Values)
        {
            var parentCount = impliedTags[parent.Hash] - 1;
            if (parentCount <= 0)
                impliedTags.Remove(parent.Hash);
            else
                impliedTags[parent.Hash] = parentCount;
        }
        return true;
    }

    public bool RemoveTags(TagSet otherSet)
    {
        if (otherSet == null) return false;
        
        bool changed = false;
        foreach (var tag in otherSet.AllTags())
        {
            changed = RemoveTag(tag) || changed;
        }
        return changed;
    }

    public void Clear()
    {
        tags.Clear();
        impliedTags.Clear();
    }

    public List<Tag> GetSubTags(Tag rootTag, bool includeRoot = false, bool includeImplied = false)
    {
        var result = new List<Tag>();
        if (includeRoot)
            result.Add(rootTag);

        foreach (var tag in AllTags())
        {
            if (rootTag.HasChild(tag))
                result.Add(tag);
        }

        if (includeImplied)
        {
            foreach (var tag in AllTags())
            {
                foreach (var parent in tag.Parents.Values)
                {
                    if (parent != rootTag && !result.Contains(parent) && rootTag.HasChild(parent))
                        result.Add(parent);
                }
            }
        }

        return result;
    }
}