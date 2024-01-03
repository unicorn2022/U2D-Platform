[TOC]

# 01、导入序列帧动画

原始动画资源：`Assets/Sprite/PlayerSheet.png`

- **Sprite模式**：多个
- **每单位像素数**：16
- **过滤模式**：点（无过滤器）

<img src="AssetMarkdown/PlayerSheet.png" alt="PlayerSheet" style="zoom:80%;" />

切割动画：

- 每个单元的尺寸：64×32像素

生成动画：

- 选择第1帧，拖入场景，生成原始人物
- 选择动画对应的多个序列帧，拖拽至人物身上，自动创建动画

# 02、2D角色移动

地面素材：`Assets/Sprite/Ground.png`

- **每单位像素数**：16
- 此处使用一整张图表示一个地面，后续会改为Unity的tilemap功能制作地图

<img src="AssetMarkdown/Ground.png" alt="Ground" style="zoom:80%;" />

为角色添加组件

- `Rigidbody 2D`：刚体组件，用于角色移动

  - **碰撞检测**：持续

  - **休眠模式**：从不休眠

  - **Constriants|冻结旋转**：冻结Z轴旋转（因为是2D游戏）

- `Capsule Collider 2D`：碰撞体组件，用于表示角色身体的碰撞
  - **是触发器**：不勾选

- `Box Collider 2D`：碰撞体组件，用于表示角色脚的碰撞
  - **是触发器**：勾选

为地面添加组件

- `Box Collider 2D`：碰撞体组件，用于表示地面
  - **是触发器**：不勾选

添加脚本：`Assets/Scripts/PlayerController`，控制角色移动

- 通过`Input.GetAxis("Horizontal")`，获取水平移动方向
- 通过设置刚体组件的速度属性`rigidbody.velocity`，控制角色移动