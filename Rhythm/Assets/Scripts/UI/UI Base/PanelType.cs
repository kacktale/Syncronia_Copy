/// <summary>
/// <para>Default : 기본적인 UI</para>
/// <para>Popup : 작게 뜨거나 작게 뜨는 UI 예) 정말로 종료하시겠습니까?</para>
/// <para>Block : 이거 열려 있으면 다른 UI 작동 X 예) 경고창</para>
/// <para>Editor : Block과 기능은 똑같습니다. 에디터 UI를 위해서 별개로 존재하는 패널 타입</para>
/// <para>Hud : 화면에 항상 붙어 있고 다른 UI 보다 약간 우선적으로 보여야 하는 UI 혹은 잠깐 작게 뜨는 창. 예) 체력바 , 스코어, 에디터 수정 표시</para>
/// </summary>
/// 
public enum PanelType
{
    Default,
    Popup,
    Block,
    Editor,
    Hud
}
