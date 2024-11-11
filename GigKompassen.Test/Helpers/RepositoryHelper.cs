using System.Text;

namespace GigKompassen.Test.Helpers
{
  public static class RepositoryHelper
  {

    public static KeyValuePair<string, KeyValuePair<bool, object>[]> GetEntry(string entryName, params (bool isValid, object value)[] values)
    {
      return new KeyValuePair<string, KeyValuePair<bool, object>[]>(
          entryName,
          values.Select(v => new KeyValuePair<bool, object>(v.isValid, v.value)).ToArray()
      );
    }

    public static IEnumerable<Dictionary<string, KeyValuePair<bool, object>>> GetCombinations(params KeyValuePair<string, KeyValuePair<bool, object>[]>[] entries)
    {
      var allValues = entries.Select(e => e.Value).ToArray();
      var combinations = GetAllCombinations(allValues);

      foreach (var combination in combinations)
      {
        var combinationDictionary = new Dictionary<string, KeyValuePair<bool, object>>();

        for (int i = 0; i < entries.Length; i++)
        {
          var entryName = entries[i].Key;
          var item = combination[i];
          combinationDictionary[entryName] = item;
        }

        yield return combinationDictionary;
      }
    }

    public static IEnumerable<KeyValuePair<bool, object>[]> GetAllCombinations(params KeyValuePair<bool, object>[][] arrays)
    {
      // Start with an empty combination
      IEnumerable<KeyValuePair<bool, object>[]> combinations = new[] { new KeyValuePair<bool, object>[0] };

      // Loop through each array and build the combinations
      foreach (var array in arrays)
      {
        combinations = combinations.SelectMany(
            currentCombination => array.Select(
                item => currentCombination.Append(item).ToArray()
            )
        );
      }

      return combinations;
    }
  }
}
