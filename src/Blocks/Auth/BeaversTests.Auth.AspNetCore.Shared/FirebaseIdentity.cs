using Newtonsoft.Json;

namespace BeaversTests.Auth.AspNetCore.Shared;

// TODO: Лучше вынести в отдельную сборку провайдера Firebase
internal class FirebaseIdentity
{
    // ReSharper disable once CollectionNeverUpdated.Global
    public required Dictionary<string, List<string>> Identities { get; init; } = new(); // TODO: Не уверен в формате
    [JsonProperty("sign_in_provider")]
    public required string ProviderType { get; init; }

    public string? GetFirstIdentityByKey(string key) =>
        Identities.TryGetValue(key, out var values) ? values.FirstOrDefault() : null;
}