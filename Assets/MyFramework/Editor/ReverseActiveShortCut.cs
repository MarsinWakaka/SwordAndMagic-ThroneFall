namespace MyFramework.Editor
{
    public class ReverseActiveShortCut : UnityEditor.Editor
    {
        [UnityEditor.MenuItem("GameObject/Reverse Active %h")]
        private static void ReverseActive()
        {
            var gos = UnityEditor.Selection.gameObjects;
            foreach (var go in gos) {
                if (go != null) {
                    go.SetActive(!go.activeSelf);
                    UnityEditor.EditorUtility.SetDirty(go);
                }
            }
        }
    }
}