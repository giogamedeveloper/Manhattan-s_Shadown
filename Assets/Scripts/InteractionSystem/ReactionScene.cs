using UnityEngine;

public class ReactionScene : Reaction
{
    [SerializeField]
    private string _sceneName;

    protected override void React()
    {
        SceneController.Instance.FadeAndLoadScene(_sceneName);
    }
}
