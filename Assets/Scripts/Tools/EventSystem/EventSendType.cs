// *************************************************************************************************************
// 创建者: 王天博
// 创建时间: 2019/09/17 16:19:35
// 功能: 
// 版 本：v 1.2.0
// *************************************************************************************************************

public enum EventSendType {

    /// <summary>
    /// 下载模型完成前
    /// </summary>
    PreDownloadModelAction,
    /// <summary>
    /// 下载模型完成后
    /// </summary>
    DownloadModelFinishAction,
    /// <summary>
    /// 从资源库拖拽加载新的预设或者模型到场景
    /// </summary>
    LoadNewObjectToScene,
    /// <summary>
    /// 鼠标点击事件（选中物体/取消选中）
    /// </summary>
    MouseClickAction,

    /// <summary>
    /// 获取贴图下载地址
    /// </summary>
    GetTextureFilePath,

    /// <summary>
    /// 刷新模型选择窗口列表
    /// </summary>
    RefreshModelSelectList,

    /// <summary>
    /// 刷新当前选中物体的面板信息
    /// </summary>
    RefreshCurSelectObjMaterialBoard,

    /// <summary>
    /// 刷新当前选择的物体上的某一个材质的属性面板
    /// </summary>
    RefreshSelectMaterialSetting,

    /// <summary>
    /// 在给物体赋值之后再初始化面板信息
    /// </summary>
    InitSelectMaterialAfterSetValue,

    /// <summary>
    /// 在给物体赋值之后再刷新面板信息
    /// </summary>
    RefreshSelectMaterialAfterSetValue,

    /// <summary>
    /// 刷新选中物体名字
    /// </summary>
    RefreshSelectObjName,


    /// <summary>
    /// 刷新json完成事件
    /// </summary>
    RefreshJsonAction,

    /// <summary>
    /// 改变物体的显隐状态
    /// </summary>
    ChangeObjActiveState,

    /// <summary>
    /// 打开/关闭弹窗
    /// </summary>
    DialogChangeStateAction,

    /// <summary>
    /// lua将要执行的事件
    /// </summary>
    GetActionByLua,
}