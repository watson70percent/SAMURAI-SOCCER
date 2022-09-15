using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SamuraiSoccer
{
    /// <summary>
    /// 保存・一時保存のデータ受け渡しに使うキー
    /// </summary>
    public class StorageKey
    {
        public const string KEY_FIELDNUMBER = nameof(KEY_FIELDNUMBER);
        public const string KEY_STAGENUMBER = nameof(KEY_STAGENUMBER);
        public const string KEY_WINORLOSE = nameof(KEY_WINORLOSE);
        public const string KEY_RESULTMESSAGE = nameof(KEY_RESULTMESSAGE);
        public const string KEY_OPPONENT_TYPE = nameof(KEY_OPPONENT_TYPE);
    }
}
