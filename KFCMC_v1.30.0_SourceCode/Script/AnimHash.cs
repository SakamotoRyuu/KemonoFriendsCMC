using UnityEngine;

public class AnimHash : SingletonMonoBehaviour<AnimHash> {

    [System.NonSerialized]
    public int[] ID;

    public enum ParamName {
        Speed, IdleType, AnimSpeed, Jump, JumpType, Landing, SideStep, StepDirection, QuickTurn, Move, Back, Run, Refresh, KnockLight, KnockHeavy,
        Dead, Attack, AttackType, AttackSpeed, RotSpeed, WalkSpeed, IdleMotion, Climb, Curve, Fear, Smile, Drown, DrownTolerance, KnockRestoreSpeed, HomeAction, Spawn, DeadSpecial, StateFriendsIdle, StateFriendsIdle1, StateFriendsIdle2, StateFriendsRefresh, StateEnemyIdle, StateIFRIdle
    }

    protected virtual void SetAnimHashID() {
        ID = new int[System.Enum.GetValues(typeof(ParamName)).Length];
        ID[(int)ParamName.Speed] = Animator.StringToHash("Speed");
        ID[(int)ParamName.IdleType] = Animator.StringToHash("IdleType");
        ID[(int)ParamName.AnimSpeed] = Animator.StringToHash("AnimSpeed");
        ID[(int)ParamName.Jump] = Animator.StringToHash("Jump");
        ID[(int)ParamName.JumpType] = Animator.StringToHash("JumpType");
        ID[(int)ParamName.Landing] = Animator.StringToHash("Landing");
        ID[(int)ParamName.SideStep] = Animator.StringToHash("SideStep");
        ID[(int)ParamName.StepDirection] = Animator.StringToHash("StepDirection");
        ID[(int)ParamName.QuickTurn] = Animator.StringToHash("QuickTurn");
        ID[(int)ParamName.Move] = Animator.StringToHash("Move");
        ID[(int)ParamName.Back] = Animator.StringToHash("Back");
        ID[(int)ParamName.Run] = Animator.StringToHash("Run");
        ID[(int)ParamName.Refresh] = Animator.StringToHash("Refresh");
        ID[(int)ParamName.KnockLight] = Animator.StringToHash("KnockLight");
        ID[(int)ParamName.KnockHeavy] = Animator.StringToHash("KnockHeavy");
        ID[(int)ParamName.Dead] = Animator.StringToHash("Dead");
        ID[(int)ParamName.Attack] = Animator.StringToHash("Attack");
        ID[(int)ParamName.AttackType] = Animator.StringToHash("AttackType");
        ID[(int)ParamName.AttackSpeed] = Animator.StringToHash("AttackSpeed");
        ID[(int)ParamName.RotSpeed] = Animator.StringToHash("RotSpeed");
        ID[(int)ParamName.WalkSpeed] = Animator.StringToHash("WalkSpeed");
        ID[(int)ParamName.IdleMotion] = Animator.StringToHash("IdleMotion");
        ID[(int)ParamName.Climb] = Animator.StringToHash("Climb");
        ID[(int)ParamName.Curve] = Animator.StringToHash("Curve");
        ID[(int)ParamName.Fear] = Animator.StringToHash("Fear");
        ID[(int)ParamName.Smile] = Animator.StringToHash("Smile");
        ID[(int)ParamName.Drown] = Animator.StringToHash("Drown");
        ID[(int)ParamName.DrownTolerance] = Animator.StringToHash("DrownTolerance");
        ID[(int)ParamName.KnockRestoreSpeed] = Animator.StringToHash("KnockRestoreSpeed");
        ID[(int)ParamName.HomeAction] = Animator.StringToHash("HomeAction");
        ID[(int)ParamName.Spawn] = Animator.StringToHash("Spawn");
        ID[(int)ParamName.DeadSpecial] = Animator.StringToHash("DeadSpecial");
        ID[(int)ParamName.StateFriendsIdle] = Animator.StringToHash("Base Layer.Locomotion.Idle");
        ID[(int)ParamName.StateFriendsIdle1] = Animator.StringToHash("Base Layer.Locomotion.Idle1");
        ID[(int)ParamName.StateFriendsIdle2] = Animator.StringToHash("Base Layer.Locomotion.Idle2");
        ID[(int)ParamName.StateFriendsRefresh] = Animator.StringToHash("Base Layer.Locomotion.Refresh");
        ID[(int)ParamName.StateEnemyIdle] = Animator.StringToHash("Base Layer.Idle");
        ID[(int)ParamName.StateIFRIdle] = Animator.StringToHash("Base Layer.Idle");
    }

    protected override void Awake () {
        if (CheckInstance()) {
            DontDestroyOnLoad(gameObject);
            SetAnimHashID();
        }
	}
}
