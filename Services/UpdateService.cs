using System.Text.Json;
using System.Text.Json.Serialization;

namespace QuitSmoke.Services;

/// <summary>
/// Comprobacion de version al arrancar (constitucion, seccion 15): consulta un manifiesto en el
/// propio repositorio y, si hay una version mas reciente que la instalada, avisa y propone
/// actualizar. Silenciosa y no bloqueante: sin red o ya al dia, no molesta.
/// </summary>
public class UpdateService
{
    public const string AppcastUrl = "https://raw.githubusercontent.com/donki/QuitSmoke/main/appcast.json";

    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(8) };
    private readonly ILocalizationService _loc;
    private readonly ILinkOpener _links;
    private readonly Func<Task<string>> _fetch;
    private readonly Func<string> _currentVersion;
    private bool _checkedThisSession;

    public UpdateService(ILocalizationService loc, ILinkOpener links)
        : this(loc, links, () => Http.GetStringAsync(AppcastUrl), () => AppInfo.Current.VersionString)
    {
    }

    public UpdateService(ILocalizationService loc, ILinkOpener links, Func<Task<string>> fetch, Func<string> currentVersion)
    {
        _loc = loc;
        _links = links;
        _fetch = fetch;
        _currentVersion = currentVersion;
    }

    public async Task CheckAndPromptAsync(IUserDialogs dialogs)
    {
        if (_checkedThisSession)
            return;
        _checkedThisSession = true;

        try
        {
            var manifest = JsonSerializer.Deserialize<Appcast>(await _fetch());
            if (manifest?.Version is null)
                return;

            var current = _currentVersion();
            if (CompareVersions(manifest.Version, current) <= 0)
                return; // ya se esta en la ultima version (o mas nueva)

            var wantsUpdate = await dialogs.ConfirmAsync(
                _loc.GetString("update_available"),
                string.Format(_loc.GetString("update_message"), manifest.Version, current),
                _loc.GetString("update_now"),
                _loc.GetString("update_later"));

            if (wantsUpdate && !string.IsNullOrWhiteSpace(manifest.Url))
                await _links.OpenAsync(manifest.Url);
        }
        catch
        {
            // Sin red o manifiesto no disponible: la comprobacion no debe molestar ni bloquear.
        }
    }

    /// <summary>Compara versiones numericas por partes ("1.10.0"). &gt;0 si a es mas nueva que b.</summary>
    public static int CompareVersions(string a, string b)
    {
        var pa = Parts(a);
        var pb = Parts(b);
        var n = Math.Max(pa.Length, pb.Length);
        for (var i = 0; i < n; i++)
        {
            var va = i < pa.Length ? pa[i] : 0;
            var vb = i < pb.Length ? pb[i] : 0;
            if (va != vb)
                return va.CompareTo(vb);
        }
        return 0;
    }

    private static int[] Parts(string v) =>
        v.Split('.').Select(p => int.TryParse(p, out var n) ? n : 0).ToArray();

    private sealed class Appcast
    {
        [JsonPropertyName("version")] public string? Version { get; set; }
        [JsonPropertyName("url")] public string? Url { get; set; }
    }
}
