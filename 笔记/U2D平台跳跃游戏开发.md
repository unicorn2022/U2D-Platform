[TOC]

# 01、导入序列帧动画

原始动画资源：`Assets/Sprite/PlayerSheet.png`

- **Sprite模式**：多个
- **每单位像素数**：16
- **过滤模式**：点（无过滤器）
- **压缩**：无

<img src="AssetMarkdown/PlayerSheet.png" alt="PlayerSheet" style="zoom:80%;" />

切割动画：

- 每个单元的尺寸：64×32像素

生成动画：

- 选择第1帧，拖入场景，生成原始人物`Player`
- 选择动画对应的多个序列帧，拖拽至人物身上，自动创建动画

# 02、2D角色移动

地面素材：`Assets/Sprite/Ground.png`

- **每单位像素数**：16
- 此处使用一整张图表示一个地面，后续会改为Unity的tilemap功能制作地图

<img src="AssetMarkdown/Ground.png" alt="Ground" style="zoom:80%;" />

设置角色的`Tag`为：Player

为角色添加组件

- `Rigidbody 2D`：刚体组件，用于角色移动

  - **碰撞检测**：持续

  - **休眠模式**：从不休眠

  - **Constriants|冻结旋转**：冻结Z轴旋转（因为是2D游戏）

- `Capsule Collider 2D`：碰撞体组件，用于表示角色身体的碰撞
  - **是触发器**：不勾选

- `Box Collider 2D`：碰撞体组件，用于表示角色脚的碰撞
  - **是触发器**：勾选
- `PlayerController`：自定义脚本，控制角色移动

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

# 07、角色攻击：动画

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

# 08、角色攻击：HitBox

为角色添加子对象：`PlayerAttack`

为`PlayerAttack`添加组件：

- `Polygon Collider 2D`，用于表示角色的攻击范围
  - **是触发器**：勾选
  - **默认不显示**
- `PlayerAttack`：自定义脚本，控制角色攻击

添加脚本：`Assets/Scripts/PlayerAttack`，控制角色攻击

> 将上一节中的攻击相关函数，迁移到子物体中

```c#
public class PlayerAttack : MonoBehaviour {
    [Tooltip("角色攻击的伤害")]
    public int damage = 1;

    private Animator animator;
    private PolygonCollider2D collider;

    private bool canAttack = true; // 角色是否可以攻击

    void Start() {
        animator = transform.parent.GetComponent<Animator>();
        collider = GetComponent<PolygonCollider2D>();
    }

    void Update() {
        Attack();
    }
    
    void Attack() {
        // 由于控制了最短攻击间隔为0.5s, 因此可以长按攻击键持续攻击
        if (Input.GetButton("Attack") && canAttack) {
            animator.SetTrigger("Attack");
            canAttack = false;
            Invoke("AttackStart", 0.35f);
            Invoke("AttackReset", 0.5f);
        }
    }
    void AttackStart() {
        collider.enabled = true;
        Invoke("AttackEnd", 0.05f);
    }
    void AttackEnd() {
        collider.enabled = false;
    }
    void AttackReset() {
        canAttack = true;
    }
}
```

# 09、角色攻击：Enemy

敌人素材：`Assets/Sprite/Bat.png`

- **Sprite模式**：多个
- **每单位像素数**：16
- **过滤模式**：点（无过滤器）
- **压缩**：无

<img src="AssetMarkdown/Bat.png" alt="Bat" style="zoom:80%;" />

切割动画：

- 每个单元的尺寸：32×32像素

生成动画：

- 选择第1帧，拖入场景，生成原始人物`EnemyBat`
- 选择动画对应的多个序列帧，拖拽至人物身上，自动创建动画

设置`EnemyBat`的`Tag`为：Enemy

为角色添加组件

- `Box Collider 2D`：碰撞体组件，用于表示蝙蝠的碰撞
  - **是触发器**：勾选
- `EnemyBat`：自定义脚本，用于控制蝙蝠

添加脚本：`Assets/Scripts/Enemy`，控制所有的敌人类

```c#
public class Enemy : MonoBehaviour {
    [Tooltip("敌人的血量")]
    public int health = 5;
    [Tooltip("敌人的伤害")]
    public int damage = 1;

    protected void Update() {
        if (health <= 0) {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int damage) {
        health -= damage;
    }
}

```

添加脚本：`Assets/Scripts/EnemyBat`，控制蝙蝠

```c#
public class EnemyBat : Enemy {
    protected void Start(){
        base.Start();
    }
    protected void Update(){
        base.Update();
    }
}
```

修改脚本：`Assets/Scripts/PlayerAttack`

```c#
private void OnTriggerEnter2D(Collider2D collision) {
    if(collision.gameObject.tag.Equals("Enemy")) {
        collision.gameObject.GetComponent<Enemy>().TakeDamage(damage);
    }
}
```

# 10、敌人：受伤后红色闪烁

原理：更改`Sprite Renderer`组件的`颜色`属性

修改脚本：`Assets/Scripts/Enemy`

```c#
public void TakeDamage(int damage) {
    health -= damage;

    // 受伤后红色闪烁
    spriteRenderer.color = Color.red;
    Invoke("ResetColor", flashTime);
}

void ResetColor() {
    spriteRenderer.color = originColor;
}
```

# 11、敌人：简单AI

功能：在一定范围内，飞到随机的位置

修改脚本：`Assets/Scripts/EnemyBat`

```c#
public class EnemyBat : Enemy {
    [Tooltip("敌人的移动速度")]
    public float speed = 2f;
    [Tooltip("敌人移动的等待时间")]
    public float startWaitTime = 1f;

    [Tooltip("敌人的移动范围: 左下角")]
    public Transform leftDownPosition;
    [Tooltip("敌人的移动范围: 右上角")]
    public Transform rightUpPosition;

    private float waitTime;             // 敌人移动的等待时间
    private Vector2 targetPosition;     // 敌人移动的目标位置
    protected void Start() {
        base.Start();
        waitTime = startWaitTime;
        targetPosition = GetRandomPosition();
    }

    protected void Update() {
        base.Update();

        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        if(Vector2.Distance(transform.position, targetPosition) < 0.1f) {
            if (waitTime <= 0) {
                targetPosition = GetRandomPosition();
                waitTime = startWaitTime;
            } else {
                waitTime -= Time.deltaTime;
            }
        }
    }

    Vector2 GetRandomPosition() {
        return new Vector2(
            Random.Range(leftDownPosition.position.x, rightUpPosition.position.x),
            Random.Range(leftDownPosition.position.y, rightUpPosition.position.y)
        );
    }
}
```

# 12、敌人：受伤粒子特效

新建粒子系统，重命名为`BloodEffect`，并创建预制体

- **旋转**：(90, 0, 0)
- **渲染器**：
  - **材质**：Sprite-Default
- **Particle System**：
  - **起始速度**：3
  - **起始颜色**：红色
  - **循环播放**：取消勾选
  - **持续时间**：1
  - **起始生命周期**：0.5
  - **起始大小**：0.1
  - **重力修改器**：0.5
  - **模拟速度**：2
- **形状**：
  - **角度**：0
  - **半径**：0.5
- 添加自定义脚本：`BloodEffect`

新建脚本：`Assets/Scripts/BloodEffect`

```c#
public class BloodEffect : MonoBehaviour {
    [Tooltip("销毁时间")]
    public float timeToDestroy = 1f;
    void Start() {
        Destroy(gameObject, timeToDestroy);
    }
}
```

修改脚本：`Assets/Scripts/Enemy`

```c#
public GameObject bloodEffect;

public void TakeDamage(int damage) {
    health -= damage;

    // 受伤后红色闪烁
    spriteRenderer.color = Color.red;
    Invoke("ResetColor", flashTime);

    // 受伤后, 生成粒子效果
    Instantiate(bloodEffect, transform.position, Quaternion.identity);
}
```

# 13、相机跟随

新建空对象：`CameraFollow`，将相机作为其子对象

- 添加自定义脚本：`CameraFollow`，控制相机跟随

新建脚本：`Assets/Scripts/CameraFollow`

```c#
public class CameraFollow : MonoBehaviour {
    [Tooltip("相机跟随的目标")]
    public Transform target;
    [Tooltip("平滑值"), Range(0, 1)]
    public float smoothing = 0.1f;

    private void LateUpdate() {
        if (target == null) return;

        // 通过插值的方式, 让相机移动到目标位置
        if (transform.position != target.position) {
            transform.position = Vector3.Lerp(transform.position, target.position, smoothing);
        }
    } 
}
```

