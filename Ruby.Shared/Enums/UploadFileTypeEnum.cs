using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ruby.Shared.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UploadFileTypeEnum
    {
        TrackAudio = 1,
        TrackCover = 2,
        UserProfileAvatar = 3
    }
}
