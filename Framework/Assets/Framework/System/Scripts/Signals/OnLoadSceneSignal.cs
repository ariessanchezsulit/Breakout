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
        public EScene Scene { get; set; }

        /// <summary>
        /// The name of the scene.
        /// </summary>
        public string SceneName { get; set; }
    }
}
