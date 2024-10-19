using MongoDB.Bson;
using System.Text.Json;

namespace Mobile
{
    public static class InternalSettings
    {

        public static ObjectId? UserId
        {
            get
            {
                var serializedValue = Preferences.Default.Get(nameof(UserId), string.Empty);
                ObjectId? deserializedValue;
                if (string.IsNullOrEmpty(serializedValue))
                {
                    return null;
                }
                deserializedValue = JsonSerializer.Deserialize<ObjectId?>(serializedValue);
                return deserializedValue;
            }
            set
            {
                var serializedValue = JsonSerializer.Serialize(value, new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });
                Preferences.Default.Set(nameof(UserId), serializedValue);
            }
        }

        public static Guid? DeviceId
        {
            get
            {
                var serializedValue = Preferences.Default.Get(nameof(DeviceId), string.Empty);
                Guid? deserializedValue;
                if (string.IsNullOrEmpty(serializedValue))
                {
                    deserializedValue = Guid.NewGuid();
                    serializedValue = JsonSerializer.Serialize(deserializedValue, new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    Preferences.Default.Set(nameof(DeviceId), serializedValue);
                }
                deserializedValue = JsonSerializer.Deserialize<Guid?>(serializedValue);
                return deserializedValue;
            }
            set
            {
                var serializedValue = JsonSerializer.Serialize(value, new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });
                Preferences.Default.Set(nameof(DeviceId), serializedValue);
            }
        }

        public static string CurrentScreen
        {
            get
            {
                var serializedValue = Preferences.Default.Get(nameof(CurrentScreen), string.Empty);
                if (string.IsNullOrEmpty(serializedValue)) return string.Empty;
                var deSerializedValue = JsonSerializer.Deserialize<string>(serializedValue);
                return deSerializedValue;
            }
            set
            {
                var serializedValue = JsonSerializer.Serialize(value, new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });
                Preferences.Default.Set(nameof(CurrentScreen), serializedValue);
            }
        }

        public static string Name
        {
            get
            {
                var serializedValue = Preferences.Default.Get(nameof(Name), string.Empty);
                if (string.IsNullOrEmpty(serializedValue)) return string.Empty;
                var deSerializedValue = JsonSerializer.Deserialize<string>(serializedValue);
                return deSerializedValue;
            }
            set
            {
                var serializedValue = JsonSerializer.Serialize(value, new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });
                Preferences.Default.Set(nameof(Name), serializedValue);
            }
        }

        public static string RefreshToken
        {
            get
            {
                var serializedValue = Preferences.Default.Get(nameof(RefreshToken), string.Empty);
                if (string.IsNullOrEmpty(serializedValue)) return string.Empty;
                var deSerializedValue = JsonSerializer.Deserialize<string>(serializedValue);
                return deSerializedValue;
            }
            set
            {
                var serializedValue = JsonSerializer.Serialize(value, new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });
                Preferences.Default.Set(nameof(RefreshToken), serializedValue);
            }
        }

        public static string UserToken
        {
            get
            {
                var serializedValue = Preferences.Default.Get(nameof(UserToken), string.Empty);
                if (string.IsNullOrEmpty(serializedValue)) return string.Empty;
                var deSerializedValue = JsonSerializer.Deserialize<string>(serializedValue);
                return deSerializedValue;
            }
            set
            {
                var serializedValue = JsonSerializer.Serialize(value, new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });
                Preferences.Default.Set(nameof(UserToken), serializedValue);
            }
        }

        public static string EmailAddress
        {
            get
            {
                var serializedValue = Preferences.Default.Get(nameof(EmailAddress), string.Empty);
                if (string.IsNullOrEmpty(serializedValue)) return string.Empty;
                try
                {
                    var deSerializedValue = JsonSerializer.Deserialize<string>(serializedValue);
                    return deSerializedValue;
                }
                catch
                {
                    return string.Empty;
                }
            }
            set
            {
                var serializedValue = JsonSerializer.Serialize(value, new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });
                Preferences.Default.Set(nameof(EmailAddress), serializedValue);
            }
        }

    }
}
