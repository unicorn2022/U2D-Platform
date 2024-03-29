[TOC]

# 01、角色：导入序列帧动画

原始动画资源：`Assets/Sprite/Player/PlayerSheet.png`

- **Sprite模式**：多个
- **每单位像素数**：16
- **过滤模式**：点（无过滤器）
- **压缩**：无

<img src="AssetMarkdown/PlayerSheet.png" alt="PlayerSheet" style="zoom:80%;" />

切割并创建动画：

- 每个单元的尺寸：64×32像素

- 选择第1帧，拖入场景，生成原始人物`Player`
- 选择动画对应的多个序列帧，拖拽至人物身上，自动创建动画

# 02、角色：2D移动

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

# 03、角色：移动动画：Idle & Run

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

# 04、角色：跳跃

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

# 05、角色：跳跃动画

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

# 06、角色：二段跳

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

# 09、敌人：蝙蝠

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

# 13、相机：跟随 CameraFollow

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

# 14、相机：震动 CameraShake

新建空对象：`CameraShake`

- 添加自定义脚本：`CameraShake`
- 设置Tag为：`CameraShake`，便于后续查找

新建动画器控制器：`Assets/Animation/MainCamera/MainCamera.controller`

新建动画：`Assets/Animation/MainCamera/Idle`

新建动画：`Assets/Animation/MainCamera/Shake`

- 进入`动画`面板录制动画：相机左右移动

修改`MainCamera.controller`的状态机

- Idle => Shake：触发器`Shake`
- Shake => Idle：退出时间0.5s，过渡时间0.25s

<img src="AssetMarkdown/image-20240104115610956.png" alt="image-20240104115610956" style="zoom:80%;" />

为`Camera`添加`Animator`组件

- 控制器设置为：`MainCamera.controller`

新建脚本：`Assets/Scripts/CameraShake`

```c#
public class CameraShake : MonoBehaviour {
    [Tooltip("相机的动画组件")]
    public Animator cameraAnimator;
    
    void Start() {
        GameController.cameraShake = GameObject.FindGameObjectWithTag("CameraShake").GetComponent<CameraShake>();
    }
    
    public void Shake() {
        cameraAnimator.SetTrigger("Shake");
    }
}
```

新建脚本：`Assets/Scripts/GameController`，用于存放全局静态变量

```c#
public class GameController : MonoBehaviour {
    [Tooltip("相机抖动组件")]
    public static CameraShake cameraShake;
}
```

修改脚本：`Assets/Scripts/Enemy`

```c#
public void TakeDamage(int damage) {
    health -= damage;

    // 受伤后红色闪烁
    spriteRenderer.color = Color.red;
    Invoke("ResetColor", flashTime);

    // 受伤后, 生成粒子效果
    Instantiate(bloodEffect, transform.position, Quaternion.identity);

    // 受伤后, 相机抖动
    GameController.cameraShake.Shake();
}
```

# 15、相机：限制移动范围

修改脚本：`Assets/Scripts/CameraFollow`

```c#
[Tooltip("相机的最小位置")]
public Vector2 minPosition;
[Tooltip("相机的最大位置")]
public Vector2 maxPosition;

private void LateUpdate() {
    if (target == null) return;

    if (transform.position != target.position) {
        Vector3 targetPosition = target.position;
        // 限定相机的移动范围
        targetPosition.x = Mathf.Clamp(targetPosition.x, minPosition.x, maxPosition.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minPosition.y, maxPosition.y);
        // 通过插值的方式, 让相机移动到目标位置
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing);
    }
}

public void SetCameraPositionLimit(Vector2 minPos, Vector2 maxPos) {
    minPosition = minPos;
    maxPosition = maxPos;
}
```

# 16、角色：受伤闪烁

为`Player`添加自定义脚本：`PlayerHealth`

新建脚本：`Assets/Scripts/PlayerHealth`，用于控制角色血量

```c#
public class PlayerHealth : MonoBehaviour {
    [Tooltip("玩家的生命值")]
    public int health = 5;
    [Tooltip("玩家受到伤害后闪烁的次数")]
    public int numBlinks = 2;
    [Tooltip("玩家受到伤害后闪烁的时间")]
    public float seconds = 0.1f;

    private Renderer playerRenderer;    // 玩家的 Renderer 组件
    
    void Start() {
        playerRenderer = GetComponent<Renderer>();
    }
    
    public void TakeDamage(int damage) {
        health -= damage;

        if (health <= 0) {
            Destroy(gameObject);
            return;
        }

        // 受伤后闪烁
        BlinkPlayer(numBlinks, seconds);
    }

    void BlinkPlayer(int numBlinks, float seconds) {
        StartCoroutine(DoBlinks(numBlinks, seconds));
    }
    IEnumerator DoBlinks(int numBlinks, float seconds) {
        for(int i = 0; i < numBlinks * 2; i++) {
            playerRenderer.enabled = !playerRenderer.enabled;
            yield return new WaitForSeconds(seconds);
        }
        playerRenderer.enabled = true;
    }
}
```

修改脚本：`Assets/Scripts/Enemy`

```c#
private PlayerHealth playerHealth;      // 玩家的生命值组件

private void OnTriggerEnter2D(Collider2D collision) {
    // 如果敌人与玩家碰撞, 并且玩家的碰撞器是胶囊体碰撞器
    if(collision.gameObject.tag == "Player" && collision.GetType().ToString() == "UnityEngine.CapsuleCollider2D") {
        if(playerHealth != null) {
            playerHealth.TakeDamage(damage);
        }
    }
}
```

# 17、角色：死亡

## 17.1	[BUG]角色跳跃至平台边缘后卡死

新建**2D|Physical Material 2D**，重命名为`PlayerFriction`

- 摩擦力设置为：`0`

将角色胶囊碰撞体的材质，设置为`PlayerFriction`

## 17.2	角色死亡

修改角色动画状态机

- Any State => Death：触发器`Death`

<img src="AssetMarkdown/image-20240104203716686.png" alt="image-20240104203716686" style="zoom:80%;" />

修改脚本：`PlayerHealth`

```c#
private Animator playerAnimator;    // 玩家的 Animator 组件

public void TakeDamage(int damage) {
    health -= damage;

    // 玩家死亡
    if (health <= 0) {
        playerAnimator.SetTrigger("Death");
        Invoke("KillPlayer", 0.9f);
        return;
    }

    // 受伤后闪烁
    BlinkPlayer(numBlinks, seconds);
}

void KillPlayer() {
    Destroy(gameObject);
}
```

# 18、Layer 和 Sorting Layer

- Layer 图层：处理**碰撞**相关
- Sorting Layer 排序图层：处理**显示**顺序

图层碰撞矩阵：**编辑|项目设置|2D 物理**

# 19、场景：2D Tile Map

原始瓦片资源：`Assets/Sprite/WallTile/WallTile0~2.png`

- **每单位像素数**：16
- **过滤模式**：点（无过滤器）
- **压缩**：无

创建**平铺调色板**，重命名为`MyPalette`

- 将上述三个瓦片资源的Sprite，拖入平铺调色板中

<img src="AssetMarkdown/image-20240104210846177.png" alt="image-20240104210846177" style="zoom:80%;" />

创建**2D对象|瓦片地图|矩形**

- `Grid`的子对象，即为`TileMap`
- 再创建一个`TileMap`，分别对应`ForeGround`和`BackGround`
  - 设置各自的图层、排序图层
  - 在调色板中选中瓦片，即可绘制
- 为`ForeGround`添加组件
  - `Tilemap Collider 2D`：添加碰撞功能
    - **由复合使用**：勾选
  - `Composite Collider 2D`：将碰撞体合并到一起
  - `Rigid Body 2D`：添加Composite Collider 2D时默认添加
    - **身体类型**：静态

# 20、UI：角色血量条

原始血条资源：`Assets/Sprite/Player/HP_MP_Bar.png`

- **Sprite模式**：多个
- **每单位像素数**：16
- **过滤模式**：点（无过滤器）
- **压缩**：无

切割动画：

- 自动切割

新建**UI|画布**，在其下新建

- **UI|图像**，重命名为`HealthBar`
  - **源图像**：`HP_MP_Bar_0`
  - 添加自定义脚本：`UIHealthBar`
- **UI|图像**，重命名为`Health`
  - **源图像**：`HP_MP_Bar_1`
  - **图像类型**：已填充
  - **填充方法**：水平
  - **填充原点**：左
- **UI|文本**，重命名为`HealthText`

新建脚本：`UIHealthBar`

```c#
public class UIHealthBar : MonoBehaviour {
    [Tooltip("生命值文本UI组件")]
    public Text healthText;
    [Tooltip("血量条UI组件")]
    public Image healthBar;

    [Tooltip("角色当前血量")]
    public static int HealthCurrent;
    [Tooltip("角色最大血量")]
    public static int HealthMax;

    void Update() {
        healthBar.fillAmount = (float)HealthCurrent / HealthMax;
        healthText.text = HealthCurrent.ToString() + "/" + HealthMax.ToString();
    }
}
```

修改脚本：`PlayerHealth`

```c#
void Start() {
    playerRenderer = GetComponent<Renderer>();
    playerAnimator = GetComponent<Animator>();

    // 初始化血量条
    UIHealthBar.HealthMax = health;
    UIHealthBar.HealthCurrent = health;
}

public void TakeDamage(int damage) {
    // 更新血量
    health -= damage;
    if (health <= 0) health = 0;
    
    // 更新血量条
    UIHealthBar.HealthCurrent = health;

    // 玩家死亡
    if (health <= 0) {
        playerAnimator.SetTrigger("Death");
        Invoke("KillPlayer", 0.9f);
        return;
    }

    // 受伤后闪烁
    BlinkPlayer(numBlinks, seconds);
}
```

# 21、UI：角色受伤屏幕红闪

## 21.1	屏幕红闪

新建**UI|图像**，重命名为`ScreenFlash`

- **颜色**： FF0000，透明度设置为0
- **锚点**：设置为四周，并将位置均设置为0

<img src="AssetMarkdown/image-20240106210212215.png" alt="image-20240106210212215" style="zoom:80%;" />

新建脚本`UIScreenFlash`，挂载到`Player`上

```c#
public class UIScreenFlash : MonoBehaviour {
    [Tooltip("屏幕红闪对应的Image组件")]  
    public Image image;
    [Tooltip("屏幕红闪的持续时间")]
    public float flashTime = 0.1f;

    private Color flashColor;   // 红闪的颜色
    private Color defaultColor; // 默认的颜色

    void Start() {
        defaultColor = image.color;
        flashColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);
    }

    
    public void FlashScreen() {
        StartCoroutine(Flash());
    }
    IEnumerator Flash() {
        image.color = flashColor;
        yield return new WaitForSeconds(flashTime);
        image.color = defaultColor;
    }
}
```

修改脚本：`PlayerHealth`

```c#
public void TakeDamage(int damage) {
    // 更新血量
    health -= damage;
    if (health <= 0) health = 0;
    
    // 更新血量条
    UIHealthBar.HealthCurrent = health;

    // 玩家死亡
    if (health <= 0) {
        playerAnimator.SetTrigger("Death");
        Invoke("KillPlayer", 0.9f);
        return;
    }

    // 受伤后闪烁
    BlinkPlayer(numBlinks, seconds);
    // 屏幕红闪
    uiScreenFlash.FlashScreen();
}
```

## 21.1	[BUG]角色死亡后仍能移动

修改脚本：`GameController`

```c#
public class GameController : MonoBehaviour {
    [Tooltip("相机抖动组件")]
    public static CameraShake cameraShake;
    [Tooltip("玩家是否存活")]
    public static bool isPlayerAlive = true;
}
```

修改脚本：`PlayerController`

```c#
void Update() {
    if (!GameController.isPlayerAlive) return;
    Run();
    Jump();
}
```

修改脚本：`PlayerHealth`

```c#
public void TakeDamage(int damage) {
    // 更新血量
    health -= damage;
    if (health <= 0) health = 0;
    
    // 更新血量条
    UIHealthBar.HealthCurrent = health;

    // 玩家死亡
    if (health <= 0) {
        playerAnimator.SetTrigger("Death");
        GameController.isPlayerAlive = false;
        // 玩家死亡后, 不能移动, 不能受重力影响
        rigidbody.velocity = Vector2.zero;
        rigidbody.gravityScale = 0;
        // 玩家死亡后, 0.9s 后销毁角色
        Invoke("KillPlayer", 0.9f);
        return;
    }

    // 受伤后闪烁
    BlinkPlayer(numBlinks, seconds);
    // 屏幕红闪
    uiScreenFlash.FlashScreen();
}
```

# 22、场景：地刺陷阱

地刺素材：`Assets/Sprite/ForeGround/Spike`

- **每单位像素数**：16
- **过滤模式**：点（无过滤器）
- **压缩**：无
- 进入**Sprite编辑器**，修改**Custom Physics Shape**，将碰撞体画在地刺上

将素材拖入平铺调色板中，创建`tilemap`

创建**2D对象|瓦片地图|矩形**，重命名为`Spike`

- **排序图层**：ForeGround
- **图层**：Spike
- 添加组件：
  - `Tilemap Collider 2D`：添加碰撞功能
    - **由复合使用**：勾选
  - `Composite Collider 2D`：将碰撞体合并到一起
    - **是触发器**：勾选
  - `Rigid Body 2D`：添加Composite Collider 2D时默认添加
    - **身体类型**：静态
  - `Spike`：自定义脚本

新建脚本：`Spike`

```c#
public class Spike : MonoBehaviour {
    [Tooltip("尖刺的伤害")]
    public int damage = 1;

    private PlayerHealth playerHealth;  // 玩家的生命值组件

    void Start() {
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player" && collision.GetType().ToString() == "UnityEngine.PolygonCollider2D") {
            if (playerHealth != null) {
                // 玩家受到伤害
                playerHealth.TakeDamage(damage);
            }
        }
    }
}
```

为`Player`添加一个`Polygon Collider 2D`：用于对角色造成伤害

修改脚本：`PlayerHealth`

```c#
public void TakeDamage(int damage) {
    // 更新血量
    health -= damage;
    // 禁用碰撞体, 实现短暂无敌效果
    polygonCollider2D.enabled = false;
    if (health <= 0) health = 0;
    
    // 更新血量条
    UIHealthBar.HealthCurrent = health;

    // 玩家死亡
    if (health <= 0) {
        playerAnimator.SetTrigger("Death");
        GameController.isPlayerAlive = false;
        // 玩家死亡后, 不能移动, 不能受重力影响
        rigidbody.velocity = Vector2.zero;
        rigidbody.gravityScale = 0;
        // 玩家死亡后, 0.9s 后销毁角色
        Invoke("KillPlayer", 0.9f);
        return;
    }

    // 受伤后闪烁
    BlinkPlayer(numBlinks, seconds);
    // 屏幕红闪
    uiScreenFlash.FlashScreen();
}

void BlinkPlayer(int numBlinks, float seconds) {
    StartCoroutine(DoBlinks(numBlinks, seconds));
}
IEnumerator DoBlinks(int numBlinks, float seconds) {
    for(int i = 0; i < numBlinks * 2; i++) {
        playerRenderer.enabled = !playerRenderer.enabled;
        yield return new WaitForSeconds(seconds);
    }
    playerRenderer.enabled = true;

    // 闪烁结束后一段时间, 恢复碰撞体
    yield return new WaitForSeconds(0.5f);
    polygonCollider2D.enabled = true;
}
```

# 23、场景：移动平台

移动平台素材：`Assets/Sprite/ForeGround/MovingPlatform`

- **每单位像素数**：16
- **过滤模式**：点（无过滤器）
- **压缩**：无

添加到场景中，重命名为`MovingPlatform`

- 添加组件：`Box Collider 2D`
- 添加自定义脚本：`MovingPlatform`

添加脚本：`MovingPlatform`

```c#
public class MovingPlatform : MonoBehaviour {
    [Tooltip("平台移动的速度")]
    public float speed = 2f;
    [Tooltip("平台到达目标点后移动的时间")]
    public float waitTime = 0.5f;
    [Tooltip("平台移动的目标点")]
    public Transform[] movePositions;

    private int i;
    private float hasWaited;
    private Transform playerDefaultParent;

    void Start() {
        i = 0;
        hasWaited = 0f;
    }

    void Update() {
        if(Vector2.Distance(transform.position, movePositions[i].position) < 0.1f) {
            hasWaited += Time.deltaTime;
            if(hasWaited >= waitTime) {
                hasWaited = 0f;
                i++;
                if(i >= movePositions.Length)  i = 0;
            }
        } else {
            transform.position = Vector2.MoveTowards(transform.position, movePositions[i].position, speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.CompareTag("Player") && collision.GetType().ToString() == "UnityEngine.BoxCollider2D") {
            playerDefaultParent = collision.transform.parent;
            collision.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if(collision.CompareTag("Player") && collision.GetType().ToString() == "UnityEngine.BoxCollider2D") {
            collision.transform.SetParent(playerDefaultParent);
        }
    }
}
```

# 24、场景：单向跳跃平台

单向跳跃素材：`Assets/Sprite/ForeGround/OneWayPlatform`

- **每单位像素数**：16
- **过滤模式**：点（无过滤器）
- **压缩**：无
- **Sprite Editor|Custom Physics Shapes**：将碰撞盒放在第一个色块上，增加容错率

将素材拖入平铺调色板中，创建`tilemap`

创建**2D对象|瓦片地图|矩形**，重命名为`OneWayPlatform`

- **排序图层**：ForeGround
- **图层**：OneWayPlatform
  - **只与Player、Enemy进行碰撞**

- 添加组件：
  - `Tilemap Collider 2D`：添加碰撞功能
    - **由复合使用**：勾选
  - `Composite Collider 2D`：将碰撞体合并到一起
    - **是触发器**：不勾选
    - **由效果器使用**：勾选
  - `Rigid Body 2D`：添加Composite Collider 2D时默认添加
    - **身体类型**：静态
  - `Platform Effector 2D`：平台效果器
    - **碰撞器遮罩**：取消勾选OneWayPlatform

> 按向下键能够离开平台：短暂将角色的碰撞层更改为`OneWayPlatform`，从而实现不碰撞
>
> - 不能禁用`Capsule Collider 2D`，防止无法与敌人碰撞
> - 必须通过`Invoke`将碰撞层改回去，因为`OneWayPlatform`不会与`OneWayPlatform`碰撞，导致`IsGround()`判断出现问题

修改脚本：`PlayerController`

```c#
void OneWayPlatformCheck() {
    if (IsGrounded()) gameObject.layer = LayerMask.NameToLayer("Player");

    if (IsOneWayPlatform() && Input.GetAxis("Vertical") < -0.1f) {
        // 将角色的碰撞层短暂改为OneWayPlatform, 使角色可以穿过单向移动平台
        gameObject.layer = LayerMask.NameToLayer("OneWayPlatform");
        // 0.5秒后, 将角色的碰撞层改回Player
        Invoke("ResetPlayerLayer", 0.5f);
    }
}

bool IsOneWayPlatform() {
    return feetCollider.IsTouchingLayers(LayerMask.GetMask("OneWayPlatform"));
}

void ResetPlayerLayer() {
    gameObject.layer = LayerMask.NameToLayer("Player");
}
```

# 25、角色：爬梯子

梯子素材：`Assets/Sprite/ForeGround/Ladder`

- **每单位像素数**：16
- **过滤模式**：点（无过滤器）
- **压缩**：无
- **Sprite Editor|Custom Physics Shapes**：将碰撞盒放中心区域

将素材拖入平铺调色板中，创建`tilemap`

创建**2D对象|瓦片地图|矩形**，重命名为`Ladder`

- **排序图层**：Ladder
- **图层**：Ladder
  - **只与Player进行碰撞**
- 添加组件：
  - `Tilemap Collider 2D`：添加碰撞功能
    - **由复合使用**：勾选
  - `Composite Collider 2D`：将碰撞体合并到一起
    - **是触发器**：勾选
  - `Rigid Body 2D`：添加Composite Collider 2D时默认添加
    - **身体类型**：静态

修改`Player`的动画状态机

- Idle => Climb：参数`Climb`为true
- Run => Climb：参数`Climb`为true
- Jump => Climb：参数`Climb`为true
- Fall => Climb：参数`Climb`为true
- Climb => Idle：参数`Climb`为false，参数`Idle`为true
- Climb => Run：参数`Climb`为false，参数`Run`为true

<img src="AssetMarkdown/image-20240107145748125.png" alt="image-20240107145748125" style="zoom:80%;" />

修改脚本：`PlayerController`

```c#
void SetPlayerStateParam() {
    // 碰撞检测参数
    isGrounded = feetCollider.IsTouchingLayers(LayerMask.GetMask("ForeGround"))
        || feetCollider.IsTouchingLayers(LayerMask.GetMask("MovingPlatform"))
        || feetCollider.IsTouchingLayers(LayerMask.GetMask("OneWayPlatform"));
    isOneWayPlatform = feetCollider.IsTouchingLayers(LayerMask.GetMask("OneWayPlatform"));
    isLadder = feetCollider.IsTouchingLayers(LayerMask.GetMask("Ladder"));

    // 动画参数
    isClimbing = animator.GetBool("Climb");
    isJumping = animator.GetBool("Jump");
    isFalling = animator.GetBool("Fall");
}

void Jump() {
    // TODO: 不能在梯子上起跳
    if (isLadder && !isGrounded) {
        animator.SetBool("Jump", false);
        animator.SetBool("Fall", false);
        return;
    }
	
    ...
}

void Climb() {
    // 角色在单向移动平台上, 不能爬梯子
    if (isOneWayPlatform) return;

    // 角色在梯子上
    if (isLadder) {
        // 角色不受重力影响
        rigidbody.gravityScale = 0f;

        float moveY = Input.GetAxis("Vertical");
        // 角色在爬梯子
        if(moveY > 0.5f || moveY < -0.5f) {
            animator.SetBool("Climb", true);
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, moveY * climbSpeed);
        } 
        // 角色从梯子上跳跃
        else if (isJumping || isFalling){
            animator.SetBool("Climb", false);
        } 
        // 角色停在梯子上
        else {
            animator.SetBool("Climb", false);
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, 0.0f);
        }
    }
    // 角色不在梯子上
    else {
        animator.SetBool("Climb", false);
        rigidbody.gravityScale = playerGravity;
    }
}
```

# 26、角色：金币掉落及拾取

金币素材：`Assets/Sprite/Item/Coin`

- **Sprite模式**：多个
- **每单位像素数**：16
- **过滤模式**：点（无过滤器）
- **压缩**：无

切割动画：

- 每个单元的尺寸：16×16像素

生成动画：

- 选择第1帧，拖入场景，生成原始金币`Coin`
- 选择动画对应的多个序列帧，拖拽至`Coin`身上，自动创建动画

## 26.1	UI：当前金币数

新建**UI|图像**，重命名为`Coin`

- **源图像**：`Coin_0`
- 添加自定义脚本：`UICoin`

**UI|文本**，重命名为`CoinText`，作为`Coin`的子对象

新建脚本：`UICoin`

```c#
public class UICoin : MonoBehaviour {
    [Tooltip("角色当前收集的金币数量")]
    public static int coinNumber;
    [Tooltip("金币的UI文本组件")]
    public Text coinText;

    void Start() {
        coinNumber = 0;
    }

    void Update() {
        coinText.text = coinNumber.ToString();
    }
}
```

## 26.2	角色：捡起金币

将金币添加到场景中

- **排序图层**：Item
- **图层**：Item
  - **只与ForeGround、MovingPlatform、OneWayPlatform进行碰撞**
- 添加组件：
  - `Box Collider 2D`
  - `Rigidbody 2D`：
    - **Constriants|冻结旋转**：冻结Z轴旋转（因为是2D游戏）

为金币添加子对象，重命名为`TriggerBox`

- **图层**：TriggerBox
  - **只与Player进行碰撞**
- 添加组件：
  - `Box Collider 2D`
    - **是触发器**：勾选
  - 自定义脚本：`Coin`

新建脚本：`Coin`

```c#
public class Coin : MonoBehaviour{
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player") && collision.GetType().ToString() == "UnityEngine.CapsuleCollider2D") {
            UICoin.coinNumber++;
            Destroy(gameObject);
        }
    }
}
```

## 26.3	敌人：掉落金币

修改脚本：`Enemy`

```c#
protected void Update() {
    if (health <= 0) {
        Instantiate(dropCoin, transform.position, Quaternion.identity);
        // 由于脚本挂载在敌人的子物体上, 所以销毁敌人的父物体
        Destroy(transform.parent.gameObject);
    }
}
```

# 27、场景：靠近提示牌显示文字对话框

## 27.1	场景：提示牌

提示牌素材：`Assets/Sprite/ForeGround/Sign`

- **每单位像素数**：16
- **过滤模式**：点（无过滤器）
- **压缩**：无

将提示牌添加到场景中

- **排序图层**：Item
- **图层**：TriggerBox
- 添加组件：
  - `Box Collider 2D`
    - **是触发器**：勾选
  - 自定义脚本：`Sign`

添加脚本：`Sign`

```c#
public class Sign : MonoBehaviour {
    [Tooltip("对话框")]
    public GameObject dialogBox;
    [Tooltip("对话框的Text组件")]
    public Text dialogBoxText;
    [Tooltip("对话框中的显示的文字")]
    public string dialogText;

    private bool isPlayerInSign = false;    // 玩家是否进入了sign范围

    void Update() {
        if(Input.GetKeyDown(KeyCode.F) && isPlayerInSign) {
            dialogBoxText.text = dialogText;
            dialogBox.SetActive(true);
        } 
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player") && collision.GetType().ToString() == "UnityEngine.CapsuleCollider2D") {
            isPlayerInSign = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player") && collision.GetType().ToString() == "UnityEngine.CapsuleCollider2D") {
            dialogBox.SetActive(false);
            isPlayerInSign = false;
        }
    }
}
```

## 27.2	UI：对话框

对话框素材：`Assets/Sprite/ForeGround/DialogBox`

- **每单位像素数**：16
- **过滤模式**：点（无过滤器）
- **压缩**：无
- **Sprite Editor**：将绿色框拖拽一下

<img src="AssetMarkdown/image-20240107165318117.png" alt="image-20240107165318117" style="zoom:80%;" />

新建**UI|图像**，重命名为`DialogBox`

- **源图像**：`DialogBox`
- 默认不显示

新建**UI|文本**，重命名为`DialogBoxText`，作为`DialogBox`的子对象

# 28、光照：呼吸灯光

## 28.1	光照

添加包：`Universal RP`

新建**渲染|URP配置文件（带2D渲染器）**，重命名为`Universal Render Pipeline Asset`

修改**项目设置|图像|可编写脚本的渲染管道设置**为：`Universal Render Pipeline Asset`

将所有对象的**渲染器|材质**，更改为：`Sprite-Lit-Default`

添加光源：

- **灯光|全局光2D**：全局光照
- **灯光|聚光灯2D**：聚光灯，可通过调整角度变为点光源
- **灯光|自由形式光源2D**：任意多边形

## 28.2	场景：火炬物体

火把素材：`Assets/Sprite/Lights/Torch`

- **Sprite模式**：多个
- **每单位像素数**：16
- **过滤模式**：点（无过滤器）
- **压缩**：无

切割动画：

- 每个单元的尺寸：16×16像素

将火把添加到场景中

- **排序图层**：Item
- 添加组件：
  - `Animator`：创建动画控制器，并录制动画，赋值给Animator组件
  - `Light 2D`：表示火把的照明范围


# 29、敌人：受伤值浮动显示

字体素材：`Assets/Sprite/Font/PixelFont_5x5`

- **渲染模式**：平滑
- **角色**：Unicode

添加第三方库脚本：`MeshRendererSortingEditor.cs`

创建空对象，重命名为`FloatPoint`，并设置为预制体

- 添加组件：
  - `Text Mesh`
    - **字符大小**：0.4
    - **字体**：PixelFont_5x5
    - **字体大小**：14
  - `Mesh Renderer`
    - **Sorting Layer**：Enemy
  - `Animator`
  - 自定义脚本：`FloatPoint`

新建动画控制器，重命名为`FloatPoint`，控制`FloatPoint`浮动

新建脚本：`FloatPoint`

```c#
public class FloatPoint : MonoBehaviour {
    [Tooltip("销毁时间")]
    public float timeToDestroy = 0.6f;
    void Start() {
        Destroy(gameObject, timeToDestroy);
    }
}
```

修改脚本：`Enemy`

```c#
public void TakeDamage(int damage) {
    health -= damage;

    // 受伤后红色闪烁
    spriteRenderer.color = Color.red;
    Invoke("ResetColor", flashTime);

    // 受伤后, 生成粒子效果
    Instantiate(bloodEffect, transform.position, Quaternion.identity);
    
    // 受伤后, 生成浮动显示伤害值
    GameObject floatpoint = Instantiate(floatPoint, transform.position, Quaternion.identity);
    floatpoint.transform.GetChild(0).GetComponent<TextMesh>().text = damage.ToString();

    // 受伤后, 相机抖动
    GameController.cameraShake.Shake();
}
```

# 30、场景：游戏背景平滑切换

> 原理：前一张图的透明度逐渐变为0，在变为0时停留一会，并切换下一张图，然后再将下一张图的透明度逐渐变为1

背景素材：`Assets/Sprite/BackGround/selda1~2`

创建物体，重命名为`BackGroundPicture`

- **父对象**：CameraFollow，从而实现背景紧跟人物
- 添加组件
  - `Sprite Renderer`：渲染图像
    - **排序图层**：BackGround
    - **图层顺序**：1
  - `Animator`：控制背景变化
  - 自定义脚本：`BackGround`

创建动画控制器，重命名为`BackGround`

- 新建动画`ChangeBackGround`：更改透明度，注意要在透明度为0停留一段时间
- Idle => ChangeBackGround：触发器`ChangeBackGround`

<img src="AssetMarkdown/image-20240107220920845.png" alt="image-20240107220920845" style="zoom:80%;" />

新建脚本：`BackGroundPicture`

```c#
public class BackGroundPicture : MonoBehaviour {
    [Header("背景图片数组")]
    public Sprite[] backGroundPictures;

    private Animator animator;              // 动画控制器
    private SpriteRenderer spriteRenderer;  // 背景图片渲染器

    public int currentBackGround;  // 当前背景图片下标
    private bool needChange;        // 是否需要切换背景图片

    void Start() {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentBackGround = 0;
        needChange = false;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.H)) {
            animator.SetTrigger("ChangeBackGround");
            needChange = true;
        }

        if(needChange && spriteRenderer.color.a == 0f) {
            currentBackGround = (currentBackGround + 1) % backGroundPictures.Length;
            spriteRenderer.sprite = backGroundPictures[currentBackGround];
            needChange = false;
        }
    }
}
```

# 31、音效：捡金币&将金币投入垃圾桶

## 31.1	场景：垃圾桶

垃圾桶素材：`Assets/Sprite/ForeGround/TrashBin`

- **每单位像素数**：12（像素数越小，在场景中看起来越大）
- **过滤模式**：点（无过滤器）
- **压缩**：无

将垃圾桶添加到场景中

- **排序图层**：Item
- **图层**：Item（与地面碰撞，与Player不碰撞）
- 添加组件：
  - `rigidbody 2D`
    - **身体类型**：静态
  - `Box Collider 2D`
  - 自定义脚本：`TrashBin`
- 添加空子对象，重命名为`TriggerBox`：与Player进行碰撞
  - **图层**：TriggerBox（仅与Enemy、Player碰撞）
  - 添加组件：`Box Collider 2D`
    - **是触发器**：勾选

新建脚本：`TrashBin`

```c#
public class TrashBin : MonoBehaviour {
    [Tooltip("垃圾桶内金币数量")]
    public int coinCurrent = 0;
    [Tooltip("垃圾桶内金币数量上限")]
    public int coinMax = 10;

    private bool isPlayerInTrashBin = false;    // 玩家是否进入了垃圾桶范围

    void Update() {
        // 按下F键投币
        if (Input.GetKeyDown(KeyCode.F) && isPlayerInTrashBin && coinCurrent < coinMax && UICoin.coinNumber > 0) {
            UICoin.coinNumber--;
            coinCurrent++;
            SoundManager.instance.PlayThrowCoin();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player") && collision.GetType().ToString() == "UnityEngine.CapsuleCollider2D") {
            isPlayerInTrashBin = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player") && collision.GetType().ToString() == "UnityEngine.CapsuleCollider2D") {
            isPlayerInTrashBin = false;
        }
    }
}
```

## 31.2	UI：跟随垃圾桶

新建**UI|图像**，重命名为`UITrashBinCoinBar`

- **源图像**：`HP_MP_Bar_0`

新建**UI|图像**，重命名为`TrashBinCoin`

- **源图像**：`HP_MP_Bar_2`
- **图像类型**：已填充
- **填充方法**：水平
- **填充原点**：左

新建**UI|文本**，重命名为`TrashBinCoinText`

新建脚本：`TrashBinUI`，添加到垃圾桶对象上

```c#
public class TrashBinUI : MonoBehaviour {
    [Tooltip("全局UI画布")]
    public RectTransform CanvasRect;

    [Tooltip("垃圾桶UI预制件")]
    public GameObject uiPrefab;
    [Tooltip("垃圾桶UI偏移")]
    public Vector2 offset = new Vector2(0, 80);


    private TrashBin trashBin;          // 垃圾桶脚本
    private RectTransform trashBinUI;   // 垃圾桶UI元素
    private Image coinNumberImage;      // 垃圾桶显示金币数量的Image组件
    private Text coinNumberText;        // 垃圾桶显示金币数量的Text组件

    void Start() {
        GameObject gameObject = Instantiate(uiPrefab, GameObject.Find("Canvas").transform);
        trashBinUI = gameObject.GetComponent<RectTransform>();
        coinNumberImage = gameObject.transform.GetChild(0).GetComponent<Image>();
        coinNumberText = gameObject.transform.GetChild(1).GetComponent<Text>();
        trashBin = GetComponent<TrashBin>();
    }

    void Update() {
        // 将垃圾桶的世界坐标转换为视口坐标
        Vector2 viewportPosition = Camera.main.WorldToViewportPoint(transform.position);
        // 将视口坐标转换为画布坐标
        Vector2 screenPosition = new Vector2(
            ((viewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)) + offset.x,
            ((viewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)) + offset.y
        );
        // 更新垃圾桶UI元素的位置
        trashBinUI.anchoredPosition = screenPosition;

        // 更新垃圾桶UI中金币数量的显示
        coinNumberImage.fillAmount = (float)trashBin.coinCurrent / trashBin.coinMax;
        coinNumberText.text = trashBin.coinCurrent + " / " + trashBin.coinMax;
    }
}
```

## 31.3	音效：捡金币&投币

音效素材：`Assets/Sprite/Resources/PickCoin、ThrowCoin`

新建空对象，重命名为`SoundManager`

- 添加组件：`Audio Source`
- 自定义脚本：`SoundManager`

新建脚本：`SoundManager`

```c#
public class SoundManager : MonoBehaviour {
    static public SoundManager instance;

    [Tooltip("音效：捡金币")]
    public AudioClip pickCoin;
    [Tooltip("音效：投金币")]
    public AudioClip throwCoin;

    private AudioSource audioSource;

    void Start() {
        instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayPickCoin() {
        if (pickCoin == null) return;
        audioSource.PlayOneShot(pickCoin);
    }

    public void PlayThrowCoin() {
        if (pickCoin == null) return;
        audioSource.PlayOneShot(throwCoin);
    }
}
```

# 32、角色：按Y扔出回旋镖

回旋镖素材：`Assets/Sprite/Player/Sickle`

- **每单位像素数**：16（像素数越小，在场景中看起来越大）
- **过滤模式**：点（无过滤器）
- **压缩**：无

将回旋镖添加到场景中

- **排序图层**：Weapon
- **图层**：Weapon（仅与Enemy、Player碰撞）
- 添加组件：
  - `rigidbody 2D`
    - **身体类型**：Kinematic
    - **碰撞检测**：连续
    - **休眠模式**：从不休眠
  - `Box Collider 2D`
    - **是触发器**：勾选
  - 自定义脚本：`Sickle`

新建脚本：`Sickle`

```c#
public class Sickle : MonoBehaviour {
    [Tooltip("飞行速度")]
    public float flySpeed = 20.0f;
    [Tooltip("旋转速度")]
    public float rotateSpeed = 30.0f;
    [Tooltip("伤害")]
    public int damage = 2;

    private Rigidbody2D rigidbody;
    private Transform playerTransform;
    private CameraShake cameraShake;
    private Vector2 startSpeed;     // 回旋镖飞出去的速度
    private float returnSpeed;      // 回旋镖回到Player身边的速度

    void Start() {
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.velocity = transform.right * flySpeed;
        startSpeed = rigidbody.velocity;
        returnSpeed = 0;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        cameraShake = GameObject.FindGameObjectWithTag("CameraShake").GetComponent<CameraShake>();
    }

    void Update() {
        // 旋转
        transform.Rotate(0, 0, -rotateSpeed);

        // 向前飞, 直到到达最远处
        if(rigidbody.velocity.magnitude > 0.1f) {
            rigidbody.velocity -= startSpeed * Time.deltaTime;
        } 
        // 到达最远处后, 开始向Player飞
        else {
            if(returnSpeed < flySpeed) returnSpeed += flySpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, returnSpeed * Time.deltaTime);
        }
        

        // 回旋镖回到Player身边时, 销毁自身
        if (Vector2.Distance(transform.position, playerTransform.position) < 0.5f) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Enemy")) {
            collision.gameObject.GetComponent<Enemy>().TakeDamage(damage);
            cameraShake.Shake();
        }
    }
}
```

新建空对象，重命名为`PlayerAttackSickle`，作为`Player`的子对象

- 添加自定义脚本：`PlayerAttackSickle`

```c#
public class PlayerAttackSickle : MonoBehaviour {
    [Tooltip("回旋镖")]
    public GameObject sickle;

    void Update() {
        if(Input.GetKeyDown(KeyCode.Q)) {
            Instantiate(sickle, transform.position, transform.rotation);
        }
    }
}
```

# 33、角色：开宝箱

宝箱素材：`Assets/Sprite/ForeGround/Item/TreasureBox`

- **Sprite模式**：多个
- **每单位像素数**：128（像素数越小，在场景中看起来越大）
- **过滤模式**：点（无过滤器）
- **压缩**：无

切割并创建动画：

- 每个单元的尺寸：300×300像素
- Idle => Opening：触发器`Open`

<img src="AssetMarkdown/image-20240110190437984.png" alt="image-20240110190437984" style="zoom:80%;" />

将宝箱添加到场景中

- **排序图层**：Item
- **图层**：Item（仅与地面碰撞）
- 添加组件：
  - `rigidbody 2D`
    - **身体类型**：Dynamic
    - **碰撞检测**：连续
    - **休眠模式**：从不休眠
  - `Box Collider 2D`
  - 自定义脚本：`TreasureBox`
- 添加空子对象，重命名为`TriggerBox`：与Player进行碰撞
  - **图层**：TriggerBox（仅与Enemy、Player碰撞）
  - 添加组件：`Box Collider 2D`
    - **是触发器**：勾选

新建脚本：`TreasureBox`

```c#
public class TreasureBox : MonoBehaviour {
    [Tooltip("宝箱中的物品")]
    public GameObject coin;

    private bool canOpen = false;
    private bool isOpened = false;
    private Animator animator;

    void Start() {
        animator = GetComponent<Animator>();
    }

    void Update() {
        // F键开启宝箱
        if (canOpen && !isOpened && Input.GetKeyDown(KeyCode.F)) {
            animator.SetTrigger("Open");
            isOpened = true;
            Invoke("GenerateCoin", 0.5f);
            Invoke("DestroyBox", 5f);
        }
    }

    void GenerateCoin() {
         Instantiate(coin, transform.position, Quaternion.identity);
    }

    void DestroyBox() {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player") && collision.GetType().ToString() == "UnityEngine.CapsuleCollider2D") {
            canOpen = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player") && collision.GetType().ToString() == "UnityEngine.CapsuleCollider2D") {
            canOpen = false;
        }
    }
}
```

将`Coin`的**图层顺序**改为`1`，从而让金币显示在宝箱前面

# 34、玩家：按U扔出炸弹

炸弹素材：`Assets/Sprite/Player/Bomb`

- **Sprite模式**：多个
- **每单位像素数**：16（像素数越小，在场景中看起来越大）
- **过滤模式**：点（无过滤器）
- **压缩**：无

切割并创建动画：

- 每个单元的尺寸：90×50像素
- Idle => Explode：触发器`Explode`

<img src="AssetMarkdown/image-20240110203246881.png" alt="image-20240110203246881" style="zoom:80%;" />

将炸弹添加到场景中

- **排序图层**：Weapon
- **图层**：Weapon（仅与Enemy、Player碰撞）
- 添加组件：
  - `rigidbody 2D`
    - **身体类型**：Dynamic
    - **碰撞检测**：连续
    - **休眠模式**：从不休眠
  - `Circle Collider 2D`
    - **是触发器**：不勾选
  - 自定义脚本：`Bomb`
  - 添加空子对象，重命名为`ExplosionRange`：与Enemy、Player进行碰撞
    - **图层**：TriggerBox（仅与Enemy、Player碰撞）
    - 添加组件：`Polygon Collider 2D`
      - **是触发器**：勾选

新建脚本：`Bomb`

```c#
public class Bomb : MonoBehaviour {
    [Tooltip("炸弹的初始速度")]
    public Vector2 startSpeed;
    [Tooltip("炸弹的伤害")]
    public int damage = 5;

    private Rigidbody2D rigidbody;
    private Animator animator;
    private GameObject explosionRange;

    void Start() {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        explosionRange = transform.GetChild(0).gameObject;
        rigidbody.velocity = transform.right * startSpeed.x + transform.up * startSpeed.y;

        Invoke("Explode", 1.5f);
    }

    void Explode() {
        animator.SetTrigger("Explode");
    }

    void StartAttack() {
        explosionRange.SetActive(true);
    }

    void DestroyBomb() {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        // 炸弹爆炸时, 对敌人造成伤害
        if (collision.gameObject.CompareTag("Enemy")) {
            collision.gameObject.GetComponent<Enemy>().TakeDamage(damage);
        }
        // 炸弹爆炸时, 对玩家造成伤害
        if (collision.gameObject.CompareTag("Player") && collision.GetType().ToString() == "UnityEngine.PolygonCollider2D") {
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }
}
```

新建空对象，重命名为`PlayerAttackBomb`，作为`Player`的子对象

- 添加自定义脚本：`PlayerAttackBomb`

```c#
public class PlayerAttackBomb : MonoBehaviour { 
    [Tooltip("炸弹")]
    public GameObject bomb;

    void Update() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            Instantiate(bomb, transform.position, transform.rotation);
        }
    }
}
```

# 35、场景：切换游戏关卡

传送门素材：`Assets/Sprite/ForeGround/Item/Door`

- **每单位像素数**：16（像素数越小，在场景中看起来越大）
- **过滤模式**：点（无过滤器）
- **压缩**：无

将传送门添加到场景中

- **排序图层**：Item
- **图层**：TriggerBox（仅与Enemy、Player碰撞）
- 添加组件：
  - `Boc Collider 2D`
    - **是触发器**：勾选
  - 自定义脚本：`DoorToNextLevel`

新建脚本：`DoorToNextLevel`

```c#
public class DoorToNextLevel : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player" && collision.GetType().ToString() == "UnityEngine.PolygonCollider2D") {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
```

在**文件|生成设置**中，将场景添加到Build中

# 36、菜单：游戏开始菜单

新建场景，重命名为`Menu`

新建**UI|画布**，在其下新建**UI|面板**，重命名为`MainMenu`，然后在其下新建

- **UI|按钮**，重命名为`PlayButton`，并设置按下的颜色
  - 添加鼠标单击事件：`Canvas`的`MainMenu`组件的`PlayGame`方法
- **UI|按钮**，重命名为`QuitButton`，并设置按下的颜色
  - 添加鼠标单击事件：`Canvas`的`MainMenu`组件的`QuitGame`方法

设置`EventSystem`的**首个选择项**为`PlayButton`

新建脚本：`MainMenu`，作为**画布**的组件

```c#
public class MainMenu : MonoBehaviour {
    public void PlayGame() {
        SceneManager.LoadScene(1);
    }

    public void QuitGame() {
        Application.Quit();
    }
}
```

新建脚本：`InitButton`，作为**MainMenu**的组件

```c#
public class InitButton : MonoBehaviour {
    private GameObject lastSelect;

    void Start() {
        lastSelect = new GameObject();
    }

    void Update() {
        if(EventSystem.current.currentSelectedGameObject == null) {
            EventSystem.current.SetSelectedGameObject(lastSelect);
        }
        else {
            lastSelect = EventSystem.current.currentSelectedGameObject;
        }
    }
}
```

# 37、菜单：按下ESC进入游戏暂停菜单

将上节课做的菜单面板`MainMenu`复制一份，重命名为`PauseMenu`，作为`Canvas`的子对象

- 将`PlayButton`更改为`ResumeButton`，表示继续游戏
- 将复制一份`Button`，更改为`MainMenuButton`，表示回到主菜单

新建脚本：`PauseMenu`，作为**画布**的组件

```c#
public class PauseMenu : MonoBehaviour {
    [Tooltip("游戏是否暂停")]
    public static bool isPaused = false;
    [Tooltip("暂停菜单UI")]
    public GameObject pauseMenuUI;

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void ResumeGame() {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1.0f;
        isPaused = false;
    }

    public void PauseGame() {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0.0f;
        isPaused = true;
    }

    public void MainMenu() {
        Time.timeScale = 1.0f;
        isPaused = false;
        SceneManager.LoadScene(0);
    }

    public void QuitGame() {
        Application.Quit();
    }
}

```

为暂停菜单的三个按钮添加相应的点击事件

# 38、UI：场景加载进度条

新建**UI|画布**，重命名为`LoadingScreen`

新建**UI|滑动条**，重命名为`LoadingSlider`，作为`LoadingScreen`的子对象

新建**UI|文本**，重命名为`LoadingText`，作为`LoadingSlider`的子对象

修改脚本：`MainMenu`（挂载在`Canvas`上）

```c#
public GameObject loadingScreen;
public Slider loadingSlider;
public Text loadingText;

/// <summary>
/// 异步加载场景
/// </summary>
/// <param name="sceneIndex">场景序号</param>
public void LoadLevel(int sceneIndex) {
    StartCoroutine(AsyncLoadLevel(sceneIndex));
}
IEnumerator AsyncLoadLevel(int sceneIndex) {
    AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
    loadingScreen.SetActive(true);

    // 等待操作完成
    while (!operation.isDone) {
        float progress = operation.progress / 0.9f; // progress的范围是[0, 0.9]
        loadingSlider.value = progress;
        loadingText.text = string.Format("{0:0}%", progress * 100);
        yield return null;
    }
}
```

# 39、Input System：支持XBox和PS4手柄

安装包：`Input System`

打开**窗口|分析|Input Debugger**，可以看到当前的输入设备

<img src="AssetMarkdown/image-20240114161022481.png" alt="image-20240114161022481" style="zoom:80%;" />

新建**Input Actions**，重命名为`PlayerInputActions`，勾选`Generate C# Class`，双击打开

- 新建**Action Maps**，重命名为`GamePlay`
- 添加**Control Scheme**，重命名为`KeyBoard`
  - 新建**Actions**，重命名为`Move`
    - **Action Type**设置为`值`，`Control Type`设置为`Vector 2`
    - 添加**Up\Down\Left\Right Composite**，重命名为`WASD`，设置四个方向分别对应`WASD`
    - 添加**Up\Down\Left\Right Composite**，重命名为`Arrow`，设置四个方向分别对应`↑↓←→`
  - 与此类似，分别创建`Jump、Attack、Bomb、Sickle、Communicate、Pause`的**Actions**

<img src="AssetMarkdown/image-20240114172439844.png" alt="image-20240114172439844" style="zoom:80%;" />

- 添加**Control Scheme**，重命名为`XBoxGamePad`
  - 分别创建`Move、Jump、Attack、Bomb、Sickle、Communicate、Pause`的**Actions**

<img src="AssetMarkdown/image-20240114172517930.png" alt="image-20240114172517930" style="zoom:80%;" />

修改所有与输入相关的脚本：

- 获取值 => `ctx.ReadValue<>()`
- 获取点击事件 => 直接调用对应的事件

`PlayerController`

```c#
#region Input System 的绑定
private PlayerInputActions controls;
private Vector2 control_move;

void Awake() {
    controls = new PlayerInputActions();

    controls.GamePlay.Move.performed += ctx => control_move = ctx.ReadValue<Vector2>();
    controls.GamePlay.Move.canceled += ctx => control_move = Vector2.zero;

    controls.GamePlay.Jump.started += ctx => Jump();
}
void OnEnable() {
    controls.GamePlay.Enable();
}
void OnDisable() {
    controls.GamePlay.Disable();
}
#endregion
```

`PlayerAttackBomb`

```c#
#region Input System 的绑定
private PlayerInputActions controls;
private Vector2 control_move;

void Awake() {
    controls = new PlayerInputActions();
    controls.GamePlay.Bomb.started += ctx => AttackBomb();
}
void OnEnable() {
    controls.GamePlay.Enable();
}
void OnDisable() {
    controls.GamePlay.Disable();
}
#endregion
```

`PlayerAttackSickle、PlayerAttack、Sign、TrashBin、TreasureBox、PauseMenu`

# 40、场景：传送门

传送门素材：`Assets/Sprite/ForeGround/Item/DoorEnter`

- **每单位像素数**：16（像素数越小，在场景中看起来越大）
- **过滤模式**：点（无过滤器）
- **压缩**：无

将传送门添加到场景中

- **排序图层**：Item
- **图层**：TriggerBox（仅与Enemy、Player碰撞）
- 添加组件：
  - `Box Collider 2D`
    - **是触发器**：勾选
  - 自定义脚本：`DoorEnter`

新建脚本：`DoorEnter`

```c#
public class DoorEnter : MonoBehaviour {
    #region Input System 的绑定
    private PlayerInputActions controls;

    void Awake() {
        controls = new PlayerInputActions();
        controls.GamePlay.Communicate.started += ctx => CommunicateWithDoor();
    }
    void OnEnable() {
        controls.GamePlay.Enable();
    }
    void OnDisable() {
        controls.GamePlay.Disable();
    }
    #endregion

    [Tooltip("传送的目标位置")]
    public Transform targetPosition;

    private bool isInDoor;
    private Transform playerTransform;

    void Start() {
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    void CommunicateWithDoor() {
        if (isInDoor) {
            playerTransform.position = targetPosition.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player") && collision.GetType().ToString() == "UnityEngine.CapsuleCollider2D") {
            isInDoor = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player") && collision.GetType().ToString() == "UnityEngine.CapsuleCollider2D") {
            isInDoor = false;
        }
    }
}
```

# 41、UI：支持多语言

下载第三方插件：`Lean Localization`

新建**Lean|Localization**，重命名为`LeanLocalizationMainMenu`

- **Languages**新增两种语言，重命名为：`Chinese`、`English`
- **Translation**新增两个目标：`PlayButton`、`ExitButton`
  - **PlayButton**的**Text**分别为：`开始游戏`、`PLAY`
  - **QuitButton**的**Text**分别为：`退出游戏`、`QUIT`

为`PlayButton`的`Text`新增组件：`Lean Localized Text`

- **Translation Name**：PlayButton

为`QuitButton`的`Text`新增组件：`Lean Localized Text`

- **Translation Name**：QuitButton

修改`LeanLocalization`的`CurrentLanguage`属性，即可实现切换语言



在菜单中，新建

- **UI|切换**，重命名为`ToggleEnglish`，将其添加到`LeanLocalizationMainMenu`中
  - 添加事件：`LeanLocalizationMainMenu`对象的`LeanLocalizatio`脚本的`SetCurrentLanguage`函数，参数为`English`
- **UI|切换**，重命名为`ToggleChinese`，将其添加到`LeanLocalizationMainMenu`中
  - 添加事件：`LeanLocalizationMainMenu`对象的`LeanLocalizatio`脚本的`SetCurrentLanguage`函数，参数为`Chinese`

新建空对象，重命名为`ChangeLanguage`

- 添加组件：`Toggle Group`
- 作为`ToggleEnglish`和`ToggleChinese`的父对象

- 将`ToggleGroup`作为`ToggleEnglish`和`ToggleChinese`的**Group**属性

# 42、敌人：追击玩家

将蝙蝠添加到场景中，重命名为`SmartBat`

- **排序图层**：Enemy
- **图层**：Enemy
- 添加组件：
  - `rigidbody 2D`
    - **身体类型**：Kinematic
    - **碰撞检测**：连续
    - **休眠模式**：从不休眠
  - `Box Collider 2D`
    - **是触发器**：勾选
  - `Animator`
    - **控制器**：`EnemyBat`
  - 自定义脚本：`EnemySmartBat`

新建脚本：`EnemySmartBat`

```c#
public class EnemySmartBat : Enemy {
    [Tooltip("敌人的移动速度")]
    public float speed = 2f;
    [Tooltip("当玩家与敌人距离一定范围以内, 敌人开始追击玩家")]
    public float radius = 15f;

    private Transform playerTransform;

    void Start() {
        base.Start();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update() {
        base.Update();

        // 如果玩家在一定范围内, 敌人开始追击玩家
        if (playerTransform != null && Vector3.Distance(transform.position, playerTransform.position) < radius) {
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, speed * Time.deltaTime);
        }
    }
}
```

# 43、玩家：按I键弓箭射击

弓箭素材：`Assets/Sprite/Player/Arrow`

- **每单位像素数**：16（像素数越小，在场景中看起来越大）
- **过滤模式**：点（无过滤器）
- **压缩**：无

将弓箭添加到场景中

- **排序图层**：Weapon
- **图层**：TriggerBox（仅与Enemy、Player碰撞）
- 添加组件：
  - `rigidbody 2D`
    - **身体类型**：Kinematic
    - **碰撞检测**：连续
    - **休眠模式**：从不休眠
  - `Box Collider 2D`
    - **是触发器**：勾选
  - 自定义脚本：`Arrow`

新建脚本：`Arrow`

```c#
public class Arrow : MonoBehaviour {
    [Tooltip("箭的移动速度")]
    public float speed = 15f;
    [Tooltip("箭的伤害值")]
    public int damage = 2;
    [Tooltip("箭的最大飞行距离")]
    public float maxDistance = 15f;

    private Rigidbody2D rigidbody;
    private Vector2 startPosition;

    void Start() {
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.velocity = transform.right * speed;
        startPosition = transform.position;
    }

    void Update() {
        if (Vector2.Distance(transform.position, startPosition) > maxDistance) {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 对敌人造成伤害
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Enemy")) {
            collision.gameObject.GetComponent<Enemy>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
```

新建空对象，重命名为`PlayerAttackArrow`，作为`Player`的子对象

- 添加自定义脚本：`PlayerAttackArrow`

```c#
public class PlayerAttackArrow : MonoBehaviour {
    #region Input System 的绑定
    private PlayerInputActions controls;

    void Awake() {
        controls = new PlayerInputActions();

        controls.GamePlay.Arrow.started += ctx => AttackArrow();
    }
    void OnEnable() {
        controls.GamePlay.Enable();
    }
    void OnDisable() {
        controls.GamePlay.Disable();
    }
    #endregion

    [Tooltip("弓箭")]
    public GameObject arrow;

    void AttackArrow() {
        Instantiate(arrow, transform.position, transform.rotation);
    }
}
```

# 44、游戏彩蛋

新建空对象：`EastEgg`

- 添加自定义脚本：`EastEgg`

```c#
public class EastEgg : MonoBehaviour {
    [Tooltip("彩蛋密码")]
    public string easterEggPassword = "2314";
    [Tooltip("目前输入的密码")]
    public static string Password;

    [Tooltip("彩蛋生成的金币的预制体")]
    public GameObject coin;
    [Tooltip("彩蛋生成的金币的数量")]
    public int coinQuantity = 20;
    [Tooltip("彩蛋生成的金币的移动速度")]
    public float coinUpSpeed = 10;
    [Tooltip("彩蛋生成的金币的间隔时间")]
    public float intervalTime = 0.1f;

    void Start() {
        Password = "";
    }

    void Update() {
        if(Password == easterEggPassword) {
            StartCoroutine(GetEnumerator());
            Password = "";
        }
    }

    IEnumerator GetEnumerator() {
        for(int i = 0; i < coinQuantity; i++) {
            GameObject gb = Instantiate(coin, transform.position, transform.rotation);
            Vector2 randomDirection = new Vector2(Random.Range(-0.3f, 0.3f), 1.0f);
            gb.GetComponent<Rigidbody2D>().velocity = randomDirection * coinUpSpeed;
            
            yield return new WaitForSeconds(intervalTime);
        }
    }
}
```

新建脚本：`EastEggID`，挂载到彩蛋触发条件对应的游戏对象上

```c#
public class EastEggID : MonoBehaviour {
    [Tooltip("彩蛋的ID")]
    public int eggID = 0;

    void OnDestroy() {
        EastEgg.Password += eggID.ToString();
    }
}
```

# 45、场景：破碎陷阱平台

破碎陷阱平台素材：`Assets/Sprite/ForeGround/TrapPlatform`

- **Sprite 模式**：多个
- **每单位像素数**：16（像素数越小，在场景中看起来越大）
- **过滤模式**：点（无过滤器）
- **压缩**：无

切割并创建动画：

- 每个单元的尺寸：48×48像素
- Idle => Collapse：触发器`Collapse`

<img src="AssetMarkdown/image-20240116193144908.png" alt="image-20240116193144908" style="zoom:80%;" />

将破碎陷阱平台添加到场景中

- **排序图层**：Item
- **图层**：MovingPlatform
- 添加组件：
  - `Boc Collider 2D`
    - **是触发器**：不勾选
  - 自定义脚本：`TrapPlatform`

新建脚本：`TrapPlatform`

```c#
public class TrapPlatform : MonoBehaviour {
    private Animator animator;
    private BoxCollider2D boxCollider;

    void Start() {
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player" && collision.GetType().ToString() == "UnityEngine.BoxCollider2D") {
            animator.SetTrigger("Collapse");
        }
    }

    void DisableBoxCollider2D() {
        boxCollider.enabled = false;
    }

    void DestroyTrapPlatform() {
        Destroy(gameObject);
    }
}
```

# 46、角色：鼠标点击发射子弹

枪素材：`Assets/Sprite/Player/Gun`

- **每单位像素数**：16（像素数越小，在场景中看起来越大）
- **过滤模式**：点（无过滤器）
- **压缩**：无

子弹素材：`Assets/Sprite/Player/Bullet`

- **每单位像素数**：16（像素数越小，在场景中看起来越大）
- **过滤模式**：点（无过滤器）
- **压缩**：无

将枪添加到场景中，作为Player的子对象

- **排序图层**：Player
- 添加空子对象，重命名为`Muzzle`，用于标记枪口的位置
- 添加自定义脚本：`Gun`

```c#
public class Gun : MonoBehaviour {
    [Tooltip("子弹预制体")]
    public GameObject bullet;
    [Tooltip("子弹发射位置")]
    public Transform muzzlePosition;

    private Vector3 mousePosition;
    private Vector2 gunDirection;

    void Update() {
        mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        gunDirection = (mousePosition - transform.position).normalized;
        float angle = Mathf.Atan2(gunDirection.y, gunDirection.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, angle);

        if(Mouse.current.leftButton.wasPressedThisFrame) {
            Instantiate(bullet, muzzlePosition.position, Quaternion.Euler(transform.eulerAngles));
        }
    }
}
```

在`Arrow`的基础上，做`Bullet`的预制体

- 修改Sprite Renderer、Box Collider2D
- 添加自定义脚本：`Bullet`

```c#
public class Bullet : MonoBehaviour {
    [Tooltip("子弹的移动速度")]
    public float speed = 15f;
    [Tooltip("子弹的伤害值")]
    public int damage = 2;
    [Tooltip("子弹的最大飞行距离")]
    public float maxDistance = 15f;

    private Rigidbody2D rigidbody;
    private Vector2 startPosition;

    void Start() {
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.velocity = transform.right * speed;
        startPosition = transform.position;
    }

    void Update() {
        if (Vector2.Distance(transform.position, startPosition) > maxDistance) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Enemy")) {
            collision.gameObject.GetComponent<Enemy>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
```

# 47、敌人：地面爬行怪物

敌人素材：`Assets/Sprite/Enemy/MiniSnake`

- **Sprite 模式**：多个
- **每单位像素数**：16（像素数越小，在场景中看起来越大）
- **过滤模式**：点（无过滤器）
- **压缩**：无

切割并创建动画：

- 每个单元的尺寸：24×24像素

将敌人添加到场景中

- **排序图层**：Enemy
- **图层**：Enemy
- 添加组件：
  - `rigidbody 2D`
    - **身体类型**：Kinematic
    - **碰撞检测**：连续
    - **休眠模式**：从不休眠
  - `Boc Collider 2D`
    - **是触发器**：勾选
  - 自定义脚本：`EnemySnake`

新建脚本：`EnemySnake`

```c#
public class EnemySnake : Enemy {
    [Tooltip("敌人的移动速度")]
    public float speed = 2f;
    [Tooltip("敌人移动的等待时间")]
    public float startWaitTime = 1f;
    [Tooltip("爬行的目标位置-左")]
    public Transform moveLeftPositions;
    [Tooltip("爬行的目标位置-右")]
    public Transform moveRightPositions;

    private bool moveRight = true;
    private float waitTime;             // 敌人移动的等待时间

    protected void Start() {
        base.Start();
        waitTime = startWaitTime;
    }

    protected void Update() {
        base.Update();
        Vector3 targetPosition = moveRight ? moveRightPositions.position : moveLeftPositions.position;
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    
        if(Vector2.Distance(transform.position, targetPosition) < 0.1f) {
            if (waitTime > 0) waitTime -= Time.deltaTime;
            else {
                // 转向
                if (moveRight) {
                    transform.eulerAngles = new Vector3(0, -180, 0);
                    moveRight = false;
                } else {
                    transform.eulerAngles = new Vector3(0, 0, 0);
                    moveRight = true;
                }
                // 重置等待时间
                waitTime = startWaitTime;
            }
        }
    }
}
```

# 48、三种延迟方法

- 协程
  - 等待的时间：`yield return new WaitForSeconds(1f);`

- Invoke
  - 调用的方法不能有参数

- Time.DeltaTime
  - 需要自己保存剩余时间的时间，在Update方法中更新等待时间

# 49、场景：隐藏地刺陷阱

隐藏地刺陷阱素材：`Assets/Sprite/ForeGround/HideSpike`

- **Sprite 模式**：多个
- **每单位像素数**：16（像素数越小，在场景中看起来越大）
- **过滤模式**：点（无过滤器）
- **压缩**：无

切割并创建动画：

- 每个单元的尺寸：16×16像素
- Idle => Attack：触发器`Attack`

<img src="AssetMarkdown/image-20240116204420522.png" alt="image-20240116204420522" style="zoom:80%;" />

将隐藏地刺陷阱添加到场景中

- **排序图层**：Spike
- **图层**：Item
- 添加组件：
  - `Boc Collider 2D`
    - **是触发器**：勾选
  - 自定义脚本：`HideSpike`

```c#
public class HideSpike : MonoBehaviour {
    [Tooltip("造成伤害的碰撞体")]
    public GameObject hideSpikeBox;

    private Animator animator;

    void Start() {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player" && collision.GetType().ToString() == "UnityEngine.PolygonCollider2D") {
            animator.SetTrigger("Attack");
            hideSpikeBox.SetActive(true);
        }
    }
}
```

为隐藏地刺陷阱添加空子对象，重命名为`HideSpikeBox`

- **图层**：TriggerBox
  - **只与Player进行碰撞**
- 添加组件：
  - `Box Collider 2D`
    - **是触发器**：勾选
  - 自定义脚本：`HideSpikeBox`

```c#
public class HideSpikeBox : MonoBehaviour {
    [Tooltip("对玩家的伤害")]
    public int damage = 1;

    private PlayerHealth playerHealth;

    void Start() {
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player" && collision.GetType().ToString() == "UnityEngine.PolygonCollider2D") {
            playerHealth.TakeDamage(damage);
        }
    }
}
```

# 50、场景：可破坏Tilemap

原始瓦片资源：`Assets/Sprite/WallTile/WallTile0.png`

新建**2D对象|瓦片地图|矩形**，重命名为`DestructableLayer`

- **图层**：DestructableLayer（只与Player、TriggerBox碰撞）
- **排序图层**：ForeGround
- 为`ForeGround`添加组件
  - `Tilemap Collider 2D`：添加碰撞功能
  - `Rigid Body 2D`：
    - **身体类型**：Kinematic
  - 自定义脚本：`DestructableLayer`

```c#
public class DestructableLayer : MonoBehaviour {
    [Tooltip("破坏层的偏移量")]
    public Vector2 offset = new Vector2(0.2f, 0.2f);

    private Tilemap destructableTilemap;
    private Vector3[] position = new Vector3[8];

    void Start() {
        destructableTilemap = GetComponent<Tilemap>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Bullet") {
            Vector3 hitPosition = collision.bounds.ClosestPoint(collision.transform.position);
            position[0] = new Vector3(hitPosition.x, hitPosition.y + offset.y, 0f);
            position[1] = new Vector3(hitPosition.x, hitPosition.y - offset.y, 0f);
            position[2] = new Vector3(hitPosition.x + offset.x, hitPosition.y , 0f);
            position[3] = new Vector3(hitPosition.x + offset.x, hitPosition.y + offset.y, 0f);
            position[4] = new Vector3(hitPosition.x + offset.x, hitPosition.y - offset.y, 0f);
            position[5] = new Vector3(hitPosition.x - offset.x, hitPosition.y, 0f);
            position[6] = new Vector3(hitPosition.x - offset.x, hitPosition.y + offset.y, 0f);
            position[7] = new Vector3(hitPosition.x - offset.x, hitPosition.y - offset.y, 0f);
        
            for(int i = 0; i < 8; i++) {
                Vector3Int pos = destructableTilemap.WorldToCell(position[i]);
                destructableTilemap.SetTile(pos, null);
            }

            Destroy(collision.gameObject);
        }
    }
}
```

# 51、场景：风力区域

风力区域素材：`Assets/Sprite/ForeGround/Wind`

- **每单位像素数**：16（像素数越小，在场景中看起来越大）
- **过滤模式**：点（无过滤器）
- **压缩**：无

将风力区域添加到场景中

- **排序图层**：ForeGround
- **图层**：TriggerBox
- 添加组件：
  - `Boc Collider 2D`
    - **是触发器**：勾选
    - **由效果器使用**：勾选
  - `Area Effector 2D`
    - **碰撞器遮罩**：只与Player交互
    - **Force|力角度**：90
    - **Force|力度**：30

# 52、场景：随即物品掉落

物品素材：`Assets/Sprite/Item/Gift/*.png`

- **每单位像素数**：16（像素数越小，在场景中看起来越大）
- **过滤模式**：点（无过滤器）
- **压缩**：无

将礼物添加到场景中

- **排序图层**：Item
- **图层**：Item
- 添加组件：
  - `rigidbody 2D`
    - **身体类型**：Dynamic
  - `Boc Collider 2D`
    - **是触发器**：不勾选

将星星添加到场景中

- **排序图层**：Item
- **图层**：TriggerBox
- **Tag**：YellowStar
- 添加组件：
  - `rigidbody 2D`
    - **身体类型**：静态
  - `Boc Collider 2D`
    - **是触发器**：勾选
  - 自定义脚本：`YellowStar`

```c#
public class YellowStar : MonoBehaviour {
    [Tooltip("礼物的预制体")]
    public GameObject[] gifts;

    public void GenerateGift() {
        int index = Random.Range(0, gifts.Length);
        Instantiate(gifts[index], transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
```

修改脚本：`PlayerAttack`

- 将`PlayerAttack`对象的**图层**修改为`TriggerBox`，并让`TriggerBox`与自己碰撞

```c#
private void OnTriggerEnter2D(Collider2D collision) {
    if (collision.gameObject.tag.Equals("Enemy")) {
        collision.gameObject.GetComponent<Enemy>().TakeDamage(damage);
    }
    if (collision.gameObject.tag.Equals("YellowStar")) {
        collision.gameObject.GetComponent<YellowStar>().GenerateGift();
    }
}
```

