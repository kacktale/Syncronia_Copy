using Unity.VisualScripting;
using UnityEngine;

public class LoadingPanel : PanelBase
{
    public override PanelType PanelType =>  PanelType.Block;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnShow()
    {
    }
    protected override void OnHide()
    {
    }
}
