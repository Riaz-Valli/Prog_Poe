public class CustomHashtable
{
    private Dictionary<string, object> _dictionary;

    public CustomHashtable()
    {
        _dictionary = new Dictionary<string, object>();
    }

    public void Add(string key, object value)
    {
        if (!_dictionary.ContainsKey(key))
        {
            _dictionary.Add(key, value);
        }
        else
        {
            _dictionary[key] = value; 
        }
    }

    public object Get(string key)
    {
        if (_dictionary.ContainsKey(key))
        {
            return _dictionary[key];
        }
        throw new KeyNotFoundException("Key not found in hashtable.");
    }

    public void Remove(string key)
    {
        if (_dictionary.ContainsKey(key))
        {
            _dictionary.Remove(key);
        }
    }

    public bool ContainsKey(string key)
    {
        return _dictionary.ContainsKey(key);
    }

    public List<KeyValuePair<string, object>> ToList()
    {
        return _dictionary.ToList();
    }
}
