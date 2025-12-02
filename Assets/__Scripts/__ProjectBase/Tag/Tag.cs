using System;
using System.Collections.Generic;
using System.Linq;

public class Tag
{
    private static Dictionary<string, Tag> _tags = new Dictionary<string, Tag>();
    
    public static Tag GetOrCreate(string name)
    {
        if (_tags.TryGetValue(name, out var existingTag))
        {
            return existingTag;
        }
        
        var newTag = new Tag(name);
        _tags[name] = newTag;
        return newTag;
    }
    
    public static Tag CreateByHash(int hash)
    {
        var tag = _tags.Values.FirstOrDefault(t => t.Hash == hash);
        return tag;
    }

    public static int GetTagHash(string name)
    {
        return name.GetHashCode();
    }
    
    public static string GetNameByHash(int hash)
    {
        var tag = CreateByHash(hash);
        return tag?.Name ?? string.Empty;
    }

    public static string GetTagSetDescription(TagSet tagSet)
    {
        if (tagSet == null) return string.Empty;
        
        var tagNames = tagSet.AllTags()
            .Select(tag => tag.Name)
            .Where(name => !string.IsNullOrEmpty(name));
            
        return string.Join(",", tagNames);
    }
    
    public int Hash { get; private set; }
    public string Name { get; private set; }
    public Dictionary<int, Tag> Parents { get; private set; } = new Dictionary<int, Tag>();
    public Dictionary<int, Tag> Children { get; private set; } = new Dictionary<int, Tag>();

    private Tag(string name)
    {
        Name = name;
        Hash = GetTagHash(name);
        
        // Handle parent tags
        if (name.Contains("."))
        {
            var parentName = name.Substring(0, name.LastIndexOf('.'));
            var parentTag = GetOrCreate(parentName);
            SetParent(parentTag);
        }
    }
    
    public override string ToString() => $"<Tag({Name}) hash:{Hash}>";
    
    public void SetParent(Tag parent)
    {
        if (!Parents.ContainsKey(parent.Hash))
        {
            Parents[parent.Hash] = parent;
            parent.SetChild(this);
        }
    }

    private void SetChild(Tag child)
    {
        if (!Children.ContainsKey(child.Hash))
        {
            Children[child.Hash] = child;
            
            // Also set all our parents as the child's parents
            foreach (var parent in Parents.Values)
            {
                child.SetParent(parent);
            }
        }
    }

    public bool HasParent(Tag other) => Parents.ContainsKey(other.Hash);
    public bool HasChild(Tag other) => Children.ContainsKey(other.Hash);

    public override int GetHashCode() => Hash;
    
    public override bool Equals(object obj)
    {
        if (obj is Tag other)
        {
            return Hash == other.Hash;
        }
        return false;
    }
}
