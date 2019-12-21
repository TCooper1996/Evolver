// GENERATED AUTOMATICALLY FROM 'Assets/PlayerControls.inputactions'

using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class PlayerControls : IInputActionCollection
{
    private InputActionAsset asset;
    public PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""2030d848-e2ab-416a-a86b-ea5299c6289b"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""cd86f4c3-35a0-41d3-958b-a893246b149a"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Rotate"",
                    ""type"": ""Value"",
                    ""id"": ""d9a8072d-fa39-47ac-a942-0aae30aafac2"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Quit"",
                    ""type"": ""Button"",
                    ""id"": ""412b651c-9ccc-4f81-a5bf-57be84ee15e5"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""378bdd3b-52aa-4006-bb4d-e9ec924b1b51"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""AbsorbPress"",
                    ""type"": ""Button"",
                    ""id"": ""13475c92-732b-4644-877a-9a348ed369b4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""AbsorbRelease"",
                    ""type"": ""Button"",
                    ""id"": ""ad8c3f5a-f885-4422-9ba7-840f74dc82de"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Reload"",
                    ""type"": ""Button"",
                    ""id"": ""d3d4b02a-d8d4-4f9f-ada7-7c66d6d8f5f9"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SelectNextEnemy"",
                    ""type"": ""Button"",
                    ""id"": ""808983be-63c9-4a82-ba7c-c09324f04779"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SelectPreviousEnemy"",
                    ""type"": ""Button"",
                    ""id"": ""ac36082a-db72-4752-9786-cd5561efe5aa"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""772d8c88-e913-4dbe-b0fe-0b5abde18a3f"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ac166289-7b71-434f-8138-efa1723cb24a"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""acc87bb6-08cf-4a9a-a69f-406ad3388bac"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Quit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e9fce9ef-9143-41d1-a5e7-d52509a08543"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""50f7e89e-8613-4712-bf0c-318f6a2a60d0"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AbsorbPress"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""19c4f1cb-3273-46b5-87df-61fef98b256a"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": ""Press(behavior=1)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AbsorbRelease"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fed890af-bb1b-4e6d-98c3-ccb7d8c3e228"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Reload"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e255a215-e7e0-4970-af10-87d1633f3f2c"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SelectNextEnemy"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e04fc448-325d-4a58-a2f4-4f64132f9a85"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SelectPreviousEnemy"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Gameplay
        m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
        m_Gameplay_Move = m_Gameplay.FindAction("Move", throwIfNotFound: true);
        m_Gameplay_Rotate = m_Gameplay.FindAction("Rotate", throwIfNotFound: true);
        m_Gameplay_Quit = m_Gameplay.FindAction("Quit", throwIfNotFound: true);
        m_Gameplay_Pause = m_Gameplay.FindAction("Pause", throwIfNotFound: true);
        m_Gameplay_AbsorbPress = m_Gameplay.FindAction("AbsorbPress", throwIfNotFound: true);
        m_Gameplay_AbsorbRelease = m_Gameplay.FindAction("AbsorbRelease", throwIfNotFound: true);
        m_Gameplay_Reload = m_Gameplay.FindAction("Reload", throwIfNotFound: true);
        m_Gameplay_SelectNextEnemy = m_Gameplay.FindAction("SelectNextEnemy", throwIfNotFound: true);
        m_Gameplay_SelectPreviousEnemy = m_Gameplay.FindAction("SelectPreviousEnemy", throwIfNotFound: true);
    }

    ~PlayerControls()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Gameplay
    private readonly InputActionMap m_Gameplay;
    private IGameplayActions m_GameplayActionsCallbackInterface;
    private readonly InputAction m_Gameplay_Move;
    private readonly InputAction m_Gameplay_Rotate;
    private readonly InputAction m_Gameplay_Quit;
    private readonly InputAction m_Gameplay_Pause;
    private readonly InputAction m_Gameplay_AbsorbPress;
    private readonly InputAction m_Gameplay_AbsorbRelease;
    private readonly InputAction m_Gameplay_Reload;
    private readonly InputAction m_Gameplay_SelectNextEnemy;
    private readonly InputAction m_Gameplay_SelectPreviousEnemy;
    public struct GameplayActions
    {
        private PlayerControls m_Wrapper;
        public GameplayActions(PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Gameplay_Move;
        public InputAction @Rotate => m_Wrapper.m_Gameplay_Rotate;
        public InputAction @Quit => m_Wrapper.m_Gameplay_Quit;
        public InputAction @Pause => m_Wrapper.m_Gameplay_Pause;
        public InputAction @AbsorbPress => m_Wrapper.m_Gameplay_AbsorbPress;
        public InputAction @AbsorbRelease => m_Wrapper.m_Gameplay_AbsorbRelease;
        public InputAction @Reload => m_Wrapper.m_Gameplay_Reload;
        public InputAction @SelectNextEnemy => m_Wrapper.m_Gameplay_SelectNextEnemy;
        public InputAction @SelectPreviousEnemy => m_Wrapper.m_Gameplay_SelectPreviousEnemy;
        public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        public void SetCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
            {
                Move.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                Move.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                Move.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                Rotate.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRotate;
                Rotate.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRotate;
                Rotate.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRotate;
                Quit.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnQuit;
                Quit.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnQuit;
                Quit.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnQuit;
                Pause.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPause;
                Pause.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPause;
                Pause.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPause;
                AbsorbPress.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAbsorbPress;
                AbsorbPress.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAbsorbPress;
                AbsorbPress.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAbsorbPress;
                AbsorbRelease.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAbsorbRelease;
                AbsorbRelease.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAbsorbRelease;
                AbsorbRelease.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAbsorbRelease;
                Reload.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnReload;
                Reload.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnReload;
                Reload.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnReload;
                SelectNextEnemy.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSelectNextEnemy;
                SelectNextEnemy.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSelectNextEnemy;
                SelectNextEnemy.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSelectNextEnemy;
                SelectPreviousEnemy.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSelectPreviousEnemy;
                SelectPreviousEnemy.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSelectPreviousEnemy;
                SelectPreviousEnemy.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSelectPreviousEnemy;
            }
            m_Wrapper.m_GameplayActionsCallbackInterface = instance;
            if (instance != null)
            {
                Move.started += instance.OnMove;
                Move.performed += instance.OnMove;
                Move.canceled += instance.OnMove;
                Rotate.started += instance.OnRotate;
                Rotate.performed += instance.OnRotate;
                Rotate.canceled += instance.OnRotate;
                Quit.started += instance.OnQuit;
                Quit.performed += instance.OnQuit;
                Quit.canceled += instance.OnQuit;
                Pause.started += instance.OnPause;
                Pause.performed += instance.OnPause;
                Pause.canceled += instance.OnPause;
                AbsorbPress.started += instance.OnAbsorbPress;
                AbsorbPress.performed += instance.OnAbsorbPress;
                AbsorbPress.canceled += instance.OnAbsorbPress;
                AbsorbRelease.started += instance.OnAbsorbRelease;
                AbsorbRelease.performed += instance.OnAbsorbRelease;
                AbsorbRelease.canceled += instance.OnAbsorbRelease;
                Reload.started += instance.OnReload;
                Reload.performed += instance.OnReload;
                Reload.canceled += instance.OnReload;
                SelectNextEnemy.started += instance.OnSelectNextEnemy;
                SelectNextEnemy.performed += instance.OnSelectNextEnemy;
                SelectNextEnemy.canceled += instance.OnSelectNextEnemy;
                SelectPreviousEnemy.started += instance.OnSelectPreviousEnemy;
                SelectPreviousEnemy.performed += instance.OnSelectPreviousEnemy;
                SelectPreviousEnemy.canceled += instance.OnSelectPreviousEnemy;
            }
        }
    }
    public GameplayActions @Gameplay => new GameplayActions(this);
    public interface IGameplayActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnRotate(InputAction.CallbackContext context);
        void OnQuit(InputAction.CallbackContext context);
        void OnPause(InputAction.CallbackContext context);
        void OnAbsorbPress(InputAction.CallbackContext context);
        void OnAbsorbRelease(InputAction.CallbackContext context);
        void OnReload(InputAction.CallbackContext context);
        void OnSelectNextEnemy(InputAction.CallbackContext context);
        void OnSelectPreviousEnemy(InputAction.CallbackContext context);
    }
}
