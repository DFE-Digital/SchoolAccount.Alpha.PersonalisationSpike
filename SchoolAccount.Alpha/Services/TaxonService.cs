using System.Reflection;

namespace SchoolAccount.Alpha.Services
{
    public interface ITaxonService
    {
        string GetTaxonName(string key);
    }

    public class TaxonService : ITaxonService
    {
        private readonly Dictionary<string, string> _taxonLookup;

        public TaxonService()
        {
            _taxonLookup = new Dictionary<string, string>();
            LoadTaxonsFromResource();
        }


        private void LoadTaxonsFromResource()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var resourceName = "SchoolAccount.Alpha.Resources.taxons.csv";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                throw new FileNotFoundException($"Embedded resource '{resourceName}' not found.");
            }

            using var reader = new StreamReader(stream);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                // Strip leading commas
                var cleanedLine = line.TrimStart(',');

                var values = cleanedLine.Split(',');
                if (values.Length >= 2)
                {
                    var firstValue = values[0].Trim();
                    var secondValue = values[1].Trim();

                    // Key is second value, Value is first value
                    if (!string.IsNullOrEmpty(secondValue))
                    {
                        _taxonLookup[secondValue] = firstValue;
                    }
                }
            }
        }

        public string GetTaxonName(string key)
        {
            return _taxonLookup.GetValueOrDefault(key, key);
        }
    }
}
