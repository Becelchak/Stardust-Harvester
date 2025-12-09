using UnityEngine;
using UnityEngine.UI;

public class BuildButton : MonoBehaviour
{
    [SerializeField] private GameObject buildPrefab;
    private Button button;

    void Start()
    {
        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(OnBuildButtonClick);
    }

    void OnBuildButtonClick()
    {
        var build = buildPrefab.GetComponent<IBuildable>();
        BuildManager.Instance.SelectBuildable(build);
    }
}