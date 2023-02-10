public class SoundDatabase : SingletonMonoBehaviour<SoundDatabase> {

    public const int musicMax = 56;
    public const int ambientMax = 23;
    
    private int[] musicCond = new int[musicMax];
    private int[] ambientCond = new int[ambientMax];
    [System.NonSerialized]
    public bool[] musicEnabled = new bool[musicMax];
    [System.NonSerialized]
    public bool[] ambientEnabled = new bool[ambientMax];
    
    protected override void Awake() {
        base.Awake();
        musicCond[0] = 0;
        musicCond[1] = 0;
        musicCond[2] = 0;
        musicCond[3] = 0;
        musicCond[4] = 0;
        musicCond[5] = 0;
        musicCond[6] = 0;
        musicCond[7] = 000105;
        musicCond[8] = 010000;
        musicCond[9] = 010204;
        musicCond[10] = 010208;
        musicCond[11] = 020000;
        musicCond[12] = 020304;
        musicCond[13] = 020308;
        musicCond[14] = 030000;
        musicCond[15] = 030404;
        musicCond[16] = 030408;
        musicCond[17] = 040000;
        musicCond[18] = 040504;
        musicCond[19] = 040508;
        musicCond[20] = 050000;
        musicCond[21] = 050604;
        musicCond[22] = 060000;
        musicCond[23] = 060704;
        musicCond[24] = 070000;
        musicCond[25] = 070804;
        musicCond[26] = 070808;
        musicCond[27] = 080000;
        musicCond[28] = 080904;
        musicCond[29] = 080908;
        musicCond[30] = 090000;
        musicCond[31] = 091004;
        musicCond[32] = 091008;
        musicCond[33] = 100000;
        musicCond[34] = 101108;
        musicCond[35] = 110000;
        musicCond[36] = 110000;
        musicCond[37] = 111210;
        musicCond[38] = 111210;
        musicCond[39] = 080901;
        musicCond[40] = 121301;
        musicCond[41] = 121306;
        musicCond[42] = 121311;
        musicCond[43] = 121316;
        musicCond[44] = 121324;
        musicCond[45] = 121329;
        musicCond[46] = 001420;
        musicCond[47] = 001424;
        musicCond[48] = 001432;
        musicCond[49] = 001432;
        musicCond[50] = 130000;
        musicCond[51] = 130000;
        musicCond[52] = 130000;
        musicCond[53] = 130000;
        musicCond[54] = 130000;
        musicCond[55] = 200000;

        ambientCond[0] = 0;
        ambientCond[1] = 0;
        ambientCond[2] = 010000;
        ambientCond[3] = 020000;
        ambientCond[4] = 030000;
        ambientCond[5] = 040000;
        ambientCond[6] = 050000;
        ambientCond[7] = 050608;
        ambientCond[8] = 060000;
        ambientCond[9] = 070000;
        ambientCond[10] = 070804;
        ambientCond[11] = 080908;
        ambientCond[12] = 100000;
        ambientCond[13] = 101108;
        ambientCond[14] = 111208;
        ambientCond[15] = 111208;
        ambientCond[16] = 111209;
        ambientCond[17] = 111210;
        ambientCond[18] = 120000;
        ambientCond[19] = 121301;
        ambientCond[20] = 121306;
        ambientCond[21] = 121311;
        ambientCond[22] = 121316;
    }

    bool CheckEnabled(int condition) {
        int condProgress = condition / 10000;
        if (condProgress == 20) {
            return (GameManager.Instance.GetClearFlag() & 2) == 2;
        } else if (condProgress <= 0 || GameManager.Instance.save.progress >= condProgress) {
            int condStage = condition % 10000 / 100;
            int condFloor = condition % 100;
            if (condStage <= 0 || GameManager.Instance.IsPlayerAnother) {
                return true;
            } else if (condStage >= GameManager.Instance.save.reachedFloor.Length) {
                return false;
            } else {
                return GameManager.Instance.save.reachedFloor[condStage] >= condFloor;
            }
        } else {
            return false;
        }
    }

    public void SetEnabled() {
        for (int i = 0; i < musicMax; i++) {
            musicEnabled[i] = CheckEnabled(musicCond[i]);
        }
        for (int i = 0; i < ambientMax; i++) {
            ambientEnabled[i] = CheckEnabled(ambientCond[i]);
        }
    }
}
