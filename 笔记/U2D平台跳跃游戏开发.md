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

# 03、角色移动动画：Idle & Run

修改角色的Animator：

- Idle => Run：参数`Run`为`true`
- Run => Idle：参数`Run`为`false`

<img src="AssetMarkdown/image-20240103165109666.png" alt="image-20240103165109666" style="zoom:80%;" />

修改脚本：`Assets/Scripts/PlayerController`，控制参数`Running`的变化，以及角色的朝向

```c#
void Run() {
    // 通过刚体组件, 控制角色移动
    float moveDir = Input.GetAxis("Horizontal"); // 水平方向的移动
    Vector2 playerVelocity = new Vector2(moveDir * runSpeed, rigidbody.velocity.y);
    rigidbody.velocity = playerVelocity;

    // 控制动画的切换
    bool playerHasXAxisSpeed = Mathf.Abs(rigidbody.velocity.x) > Mathf.Epsilon;
    animator.SetBool("Run", playerHasXAxisSpeed);
    animator.SetBool("Idle", !playerHasXAxisSpeed);

    // 控制是否需要翻转角色
    if (playerHasXAxisSpeed) {
        if (rigidbody.velocity.x > 0.1f) {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        if (rigidbody.velocity.x < -0.1f) {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
    }
}
```

# 04、角色跳跃

设置地面的layer为：`Ground`

修改脚本：`Assets/Scripts/PlayerController`

```c#
void Jump() {
    if (Input.GetButtonDown("Jump") && IsGrounded()) {
        Vector2 jumpVelocity = new Vector2(0.0f, jumpSpeed);
        rigidbody.velocity = Vector2.up * jumpVelocity;
    }
}

bool IsGrounded() {
    return feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
}
```

# 05、角色跳跃动画：Jump & Fall & Idle & Run

修改角色的Animator：

- Idle => Jump：参数`Jump`为`true`
- Jump => Fall：参数`Jump`为`false`，参数`Fall`为`true`
- Fall => Idle：参数`Fall`为`false`，参数`Idle`为`true`
- Run => Jump：参数`Jump`为`true`
- Fall => Run：参数`Fall`为`false`，参数`Run`为`true`

<img src="AssetMarkdown/image-20240103172825345.png" alt="image-20240103172825345" style="zoom:80%;" />

修改脚本：`Assets/Scripts/PlayerController`

```c#
void Jump() {
    if (Input.GetButtonDown("Jump") && IsGrounded()) {
        // 通过刚体组件, 控制角色跳跃
        Vector2 jumpVelocity = new Vector2(0.0f, jumpSpeed);
        rigidbody.velocity = Vector2.up * jumpVelocity;

        // 控制动画的切换
        animator.SetBool("Jump", true);
    }

    // 纵向速度小于0, 表示角色正在下落
    if(rigidbody.velocity.y < 0.0f) {
        animator.SetBool("Jump", false);
        animator.SetBool("Fall", true);
    }

    // 角色落地后, 重置动画
    if(IsGrounded()) {
        animator.SetBool("Fall", false);
    }
}
```

# 06、角色二段跳

修改角色的Animator：

- Fall => Jump：参数`Jump`为`true`

> 由于二段跳动画和一段跳相同，因此没有添加二段跳的状态机

<img src="AssetMarkdown/image-20240103173955138.png" alt="image-20240103173955138" style="zoom:80%;" />

修改脚本：`Assets/Scripts/PlayerController`

```c#
void Jump() {
    if (Input.GetButtonDown("Jump")) {
        // 在地面上, 进行一段跳
        if (IsGrounded()) {
            // 通过刚体组件, 控制角色跳跃
            Vector2 jumpVelocity = new Vector2(0.0f, jumpSpeed);
            rigidbody.velocity = Vector2.up * jumpVelocity;

            // 可以进行二段跳
            canDoubleJump = true;
        
            // 控制动画的切换
            animator.SetBool("Jump", true);
            animator.SetBool("Fall", false);
        } 
        // 在空中, 但可以进行二段跳
        else if (canDoubleJump) {
            Vector2 doubleJumpVelocity = new Vector2(0.0f, doubleJumpSpeed);
            rigidbody.velocity = Vector2.up * doubleJumpVelocity;

            // 不可以进行二段跳
            canDoubleJump = false;

            // 控制动画的切换
            animator.SetBool("Jump", true);
            animator.SetBool("Fall", false);
        }
    }

    // 纵向速度小于0, 表示角色正在下落
    if(rigidbody.velocity.y < 0.0f) {
        animator.SetBool("Jump", false);
        animator.SetBool("Fall", true);
    }

    // 角色落地后, 重置动画
    if(IsGrounded()) {
        animator.SetBool("Fall", false);
    }
}
```

# 07、角色攻击动画

添加按键：`编辑|项目设置|输入管理器`

- 修改大小为：`19`
- 添加按键：`Attack`，肯定按钮为`j`

修改角色的Animator：

- Any State => Attack：触发器`Attack`
- Attack => Idle：参数`Idle`为`true`，参数`Jump`为`false`，参数`Fall`为`false`，**有退出时间**
- Attack => Run：参数`Run`为`true`，参数`Jump`为`false`，参数`Fall`为`false`，**有退出时间**
- Attack => Jump：参数`Jump`为`true`，**有退出时间**
- Attack => Fall：参数`Fall`为`true`，**有退出时间**

<img src="AssetMarkdown/image-20240103175150437.png" alt="image-20240103175150437" style="zoom:80%;" />

修改脚本：`Assets/Scripts/PlayerController`

```c#
private bool canAttack = true; // 角色是否可以攻击

void Attack() {
    // 由于控制了最短攻击间隔为0.5s, 因此可以长按攻击键持续攻击
    if(Input.GetButton("Attack") && canAttack) {
        animator.SetTrigger("Attack");
        canAttack = false;
        Invoke("AttackReset", 0.5f);
    }
}

void AttackReset() {
    canAttack = true;
}
```



