# ECFSM

一个轻量的 Unity Entity–Component–FSM 游戏核心结构。思路是：**Entity 统合组件，组件用字典存数据，状态机以「状态号 + 委托」的形式写在 Data（ScriptableObject）里**——用 `<string, object>` 字典代替一大堆类和复杂继承，让状态逻辑的编码风格接近脚本语言。

## 特性

- **脚本式变量字典**：值类型走 ref 返回值（`SetRef` / `GetRef`），拿到 ref 直接当局部变量读写，改的就是字典里那份；引用类型直接存取对象
- **全程内存安全**：不再保存裸指针、不再手动 pin GC 对象，整个框架不需要 unsafe 编译选项，也不存在「指针被 GC 释放后崩溃」一类问题
- **状态 = 纯数据 + 委托**：`onEnter` / `onUpdate` / `onFixedUpdate` / `onExit` / `onAnimationEvent`，挂在 ScriptableObject 上，一份 Data 可以被多个角色共用
- **Inspector 可配**：依赖 Odin（仓库已内置）序列化字典，注入物体、初始变量、状态都能在编辑器里配
- **自带第三人称示例**：基于 Unity StarterAssets 的待机/移动/跳跃三状态 demo

## 结构

```
Entity
 └ Com
    └ Data（FSM）
       └ 状态机里访问的 GameObject / 组件
```

- **Entity**：持有 `coms` 列表，负责把 `Start` / `Update` / `FixedUpdate` 分发给每个 Com
- **Com**：变量字典的宿主，记录当前状态号 `stateno`，驱动 Data 里的状态机
- **Data**：`SerializedScriptableObject`，存放 `states` 字典和注入物体；状态逻辑只写一次，多个角色共享
- 状态机内部通过变量字典访问 Com / Entity / 注入物体；**跨 Com 访问数据还未实现**

## 快速上手

1. 根物体挂 `Entity`，把 `Com` 挂上（并加入 Entity 的 coms 列表，或互相引用）
2. 新建一个继承 `Data` 的 ScriptableObject（参考 `Data/Testman/Testman.cs`），在 `OnEnable` 里注册状态
3. Com 的 `data` 引用这个资产；`injectObject` 里注入 transform / animator / characterController 等物体
4. 运行：Entity.Start 触发当前状态 `onEnter`，之后每帧 `onUpdate`、每个物理帧 `onFixedUpdate`

状态注册示例（节选自 demo）：

```csharp
public class Testman : Data
{
    public float MoveSpeed = 2.0f;

    private void OnEnable()   { BuildStates(); }  // OnEnable 在真机上也会执行
    private void OnValidate() { BuildStates(); }

    private void BuildStates()
    {
        states[10] = new State
        {
            onEnter = _V => { Debug.Log("待机/移动：10"); },
            onUpdate = _V =>
            {
                _V.Get("transform", out Transform transform);
                _V.Get("characterController", out CharacterController cc);
                ref float _speed = ref _V.GetRef<float>("_speed");
                _speed = Mathf.Lerp(_speed, targetSpeed, Time.deltaTime * SpeedChangeRate);

                if (Input.GetKey(KeyCode.Space))
                    _V.Get<Com>("_com").ChangeState(30);
            },
        };
    }
}
```

## 变量字典 API（DictPlus）

字典里存的是 `Variable<T>` 箱子；值类型拿 ref 直改，引用类型直接拿对象。

| API | 说明 |
| --- | --- |
| `_V.SetRef(key, value)` | 写/建值类型变量，返回 `ref V` |
| `ref V v = ref _V.GetRef<V>(key)` | 读值类型，返回 `ref V`，改 v 即改字典 |
| `_V.Set(key, obj)` | 写/建引用类型，返回对象本身 |
| `_V.Get<V>(key)` / `_V.Get(key, out V v)` | 读引用类型，不存在返回 null |
| `_V.Del(key)` | 删除变量 |

```csharp
// 值类型：拿到 ref 后当普通局部变量用
_V.SetRef("_speed", 0.0f);
ref float _speed = ref _V.GetRef<float>("_speed");
_speed += 1.0f;

// 引用类型：直接拿对象
_V.Set("boxCollider", gameObject.AddComponent<BoxCollider>());
_V.Get("animator", out Animator animator);
```

变量字典的内容有三个来源：

- 状态逻辑里随时 `Set` / `SetRef`；
- `Com.Awake` 把 `injectObject`（UnityObject）和 `persistentVariables`（Inspector 里配的初始值，需要 Odin）写进字典；
- `Awake` 还会自动写入 `_com` / `_entity` / `_data` 三个入口，状态里随时可取。

## 注意

- **先 Set 再 GetRef**：`GetRef` 的键不存在会抛 `KeyNotFoundException`（异常信息里带键名），拼错键名会立刻炸出来，而不是悄悄拿个空指针
- **拿到 ref 之后别 Del 这个键**：删了以后再写 ref，会写进已经移除的箱子
- 变量字典标记了 `[NonSerialized]`，只存在于运行时；需要初始值的用 `persistentVariables` 配
- 需要 Unity 2020.2+（用到了 C# 8 的 switch 表达式）；Odin 已内置（来自 [JKFrame](https://github.com/Joker-YF/JKFrame/tree/main/JKFrame/Plugins/Sirenix)）
- 动画事件由 `Event` 组件转发到当前状态的 `onAnimationEvent`；`OnFootstep` / `OnLand` 方法名被动画剪辑引用，**别改名**

## Demo

SampleScene：Testman（StarterAssets 第三人称模型 + 动画）

| 状态 | 行为 |
| --- | --- |
| 0 初始化 | 写默认变量（速度/重力/接地等），随后跳状态 10 |
| 10 待机/移动 | 加减速、朝向平滑、接地检测；按 Space 进状态 30；动画事件播落地音效 |
| 30 空中 | 重力积分，落地回状态 10 |
