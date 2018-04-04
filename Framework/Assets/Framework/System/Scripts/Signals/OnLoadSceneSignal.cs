namespace Framework
{
    /// <summary>
    /// The given scene has started loading.
    /// </summary>
    public class OnLoadSceneSignal
    {
        /// <summary>
        /// The ID of the scene.
        /// </summary>
        public EScene SceneName { get; set; }
    }
}
