using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using System.Collections.Concurrent;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace EduAdvisor.Infrastructure.Localizer
{
    public class JsonStringLocalizer : IStringLocalizer
    {
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static readonly ConcurrentDictionary<string, Dictionary<string, string>> _cache = new();
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All)
        };

        public JsonStringLocalizer(
            IWebHostEnvironment env,
            IHttpContextAccessor httpContextAccessor)
        {
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        private Dictionary<string, string> LoadResources()
        {
            var rawCulture = _httpContextAccessor.HttpContext?
                .Request.Headers["Accept-Language"]
                .ToString()
                .Split(',').FirstOrDefault();

            var culture = rawCulture?.Split('-').FirstOrDefault()?.Trim().ToLower();
            culture = string.IsNullOrWhiteSpace(culture) ? "en" : culture;

            var cacheKey = culture;
            if (_cache.TryGetValue(cacheKey, out var cached))
                return cached;

            var path = Path.Combine(
                _env.ContentRootPath,
                "Resources",
                culture,
                "SharedResource.json");

            if (!File.Exists(path))
            {
                path = Path.Combine(
                    _env.ContentRootPath,
                    "Resources",
                    "en",
                    "SharedResource.json");
            }

            var dict = new Dictionary<string, string>();
            if (File.Exists(path))
            {
                try
                {
                    // قراءة الملف مع ترميز UTF-8 محدد
                    var json = File.ReadAllText(path, Encoding.UTF8);

                    // تنظيف JSON قبل التحليل
                    json = CleanJsonString(json);

                    dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json, _jsonOptions) ?? new();
                }
                catch (JsonException ex)
                {
                    // تسجيل الخطأ للتصحيح
                    Console.WriteLine($"JSON Error in {path}: {ex.Message}");

                    // محاولة قراءة الملف كـ text عادي كحل بديل
                    try
                    {
                        var lines = File.ReadAllLines(path, Encoding.UTF8);
                        foreach (var line in lines)
                        {
                            if (line.Contains(':'))
                            {
                                var parts = line.Split(':', 2);
                                if (parts.Length == 2)
                                {
                                    var key = parts[0].Trim().Trim('"');
                                    var value = parts[1].Trim().Trim('"', ',', ' ');
                                    dict[key] = value;
                                }
                            }
                        }
                    }
                    catch { }
                }
            }

            _cache.TryAdd(cacheKey, dict);
            return dict;
        }

        private string CleanJsonString(string json)
        {
            if (string.IsNullOrEmpty(json))
                return json;

            // إزالة التعليقات (// و /* */)
            var cleaned = System.Text.RegularExpressions.Regex.Replace(json, @"//.*?$|/\*.*?\*/", "",
                System.Text.RegularExpressions.RegexOptions.Multiline);

            // إزالة الفواصل الزائدة
            cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @",\s*}", "}");
            cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @",\s*]", "]");

            // إزالة أحرف التحكم غير المسموحة
            cleaned = new string(cleaned.Where(c => !char.IsControl(c) || c == '\n' || c == '\r' || c == '\t').ToArray());

            return cleaned;
        }

        public LocalizedString this[string name]
        {
            get
            {
                var data = LoadResources();
                return data.TryGetValue(name, out var value)
                    ? new LocalizedString(name, value, false)
                    : new LocalizedString(name, name, true);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var data = LoadResources();
                if (data.TryGetValue(name, out var value))
                {
                    try
                    {
                        return new LocalizedString(name, string.Format(value, arguments), false);
                    }
                    catch (FormatException)
                    {
                        return new LocalizedString(name, value, false);
                    }
                }
                return new LocalizedString(name, name, true);
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
            => LoadResources().Select(x => new LocalizedString(x.Key, x.Value, false));

        public IStringLocalizer WithCulture(CultureInfo culture) => this;
    }
}