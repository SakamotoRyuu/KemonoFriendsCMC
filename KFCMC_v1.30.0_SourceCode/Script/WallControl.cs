using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class WallControl : MonoBehaviour {

    //ten key position - bit flag
    //2 - 1
    //4 - 2
    //6 - 4
    //8 - 8
    //1 - 16
    //3 - 32
    //7 - 64
    //9 - 128

    [System.Serializable]
    public class Outside2468 {
        public GameObject flat;
        public GameObject face;
    }

    [System.Serializable]
    public class Outside1379 {
        public GameObject flat;
        public GameObject faceL;
        public GameObject faceR;
        public GameObject edge;
        public GameObject corner;
    }

    [System.Serializable]
    public class WallTree {
        public GameObject prefab;
        public bool forInside = false;
        public float probability = 0;
        public Vector3 offset;
    }

    public enum WallType { Normal, Special };

    public bool isSimpleVertical;
    public bool isBreakable = true;
    public WallType wallType;
    public bool wallFadeEnabled;
    public Transform wallsParent;
    public Material material;
    public Material materialForEnd;
    public WallTree[] wallTree;
    public GameObject breakEffect;

    public GameObject w_all;
    public GameObject w_2_faceCombo;
    public GameObject w_4_faceCombo;
    public GameObject w_6_faceCombo;
    public GameObject w_8_faceCombo;
    public GameObject w_end;
    public GameObject w_5;
    public Outside2468 w_2;
    public Outside2468 w_4;
    public Outside2468 w_6;
    public Outside2468 w_8;
    public Outside1379 w_1;
    public Outside1379 w_3;
    public Outside1379 w_7;
    public Outside1379 w_9;

    [System.NonSerialized]
    public Vector2Int mapPosition;
    [System.NonSerialized]
    public int wallFlag;

    GameObject objTemp;
    GameObject objTemp2;
    Renderer rendTemp;
    Renderer rendTemp2;

    private void Awake() {
        if (wallsParent == null) {
            wallsParent = transform;
        }
    }

    void SetInstance(ref GameObject obj, bool rotate = false, float rotY = 0, bool isEnd = false) {
        if (rotate) {
            objTemp = Instantiate(obj, wallsParent.position, Quaternion.Euler(new Vector3(0, rotY, 0)), wallsParent);
        } else {
            objTemp = Instantiate(obj, wallsParent);
        }
        if (!isEnd && material) {
            rendTemp = objTemp.GetComponent<Renderer>();
            if (rendTemp) {
                rendTemp.material = material;
                if (wallFadeEnabled) {
                    rendTemp.shadowCastingMode = ShadowCastingMode.Off;
                    objTemp2 = Instantiate(obj, wallsParent);
                    rendTemp2 = objTemp2.GetComponent<Renderer>();
                    if (rendTemp2) {
                        rendTemp2.material = material;
                        rendTemp2.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                        rendTemp2.receiveShadows = false;
                        int childCount = objTemp2.transform.childCount;
                        if (childCount > 0) {
                            for (int i = childCount - 1; i >= 0; i--) {
                                Destroy(objTemp2.transform.GetChild(i).gameObject);
                            }
                        }
                        objTemp2.SetActive(true);
                    }
                }
            }
        } else if (isEnd && materialForEnd) {
            rendTemp = objTemp.GetComponent<Renderer>();
            if (rendTemp) {
                rendTemp.material = materialForEnd;
            }
        }
        if (isEnd) {
            Collider colTemp = objTemp.GetComponent<Collider>();
            if (colTemp) {
                colTemp.enabled = false;
            }
            NavMeshObstacle obsTemp = objTemp.GetComponent<NavMeshObstacle>();
            if (obsTemp) {
                obsTemp.enabled = false;
            }
        }
        objTemp.SetActive(true);
    }

    public void DestroyChild() {
        int count = wallsParent.childCount;
        if (count > 0) {
            for (int i = count - 1; i >= 0 ; i--) {
                Destroy(wallsParent.GetChild(i).gameObject);
            }
        }
    }

    public void DestroyAndReformAround() {
        if (StageManager.Instance && StageManager.Instance.dungeonController) {
            StageManager.Instance.dungeonController.DigMap(mapPosition);
            EmitBreakEffect();
        }
    }

    public void EmitBreakEffect() {
        if (breakEffect) {
            Instantiate(breakEffect, wallsParent.position, wallsParent.rotation);
        }
    }

    public void EmitBreakEffectParent(ref Transform parent) {
        if (breakEffect) {
            AutoDestroy autoDestroy = Instantiate(breakEffect, wallsParent.position, wallsParent.rotation, parent).GetComponent<AutoDestroy>();
            if (autoDestroy) {
                autoDestroy.enabled = false;
            }
        }
    }

    public void SetWall(int flag) {
        wallFlag = flag;
        if (isSimpleVertical) {
            if ((flag & 1) == 0) {
                if (w_2_faceCombo) {
                    SetInstance(ref w_2_faceCombo);
                }
            }
            if ((flag & 2) == 0) {
                if (w_4_faceCombo) {
                    SetInstance(ref w_4_faceCombo);
                }
            }
            if ((flag & 4) == 0) {
                if (w_6_faceCombo) {
                    SetInstance(ref w_6_faceCombo);
                }
            }
            if ((flag & 8) == 0) {
                if (w_8_faceCombo) {
                    SetInstance(ref w_8_faceCombo);
                }
            }
        } else {
            if (flag == 255) {
                if (w_all) {
                    SetInstance(ref w_all);
                }
            } else if ((flag & 206) == 206 && (flag & 1) == 0) {
                if (w_2_faceCombo) {
                    SetInstance(ref w_2_faceCombo);
                }
            } else if ((flag & 173) == 173 && (flag & 2) == 0) {
                if (w_4_faceCombo) {
                    SetInstance(ref w_4_faceCombo);
                }
            } else if ((flag & 91) == 91 && (flag & 4) == 0) {
                if (w_6_faceCombo) {
                    SetInstance(ref w_6_faceCombo);
                }
            } else if ((flag & 55) == 55 && (flag & 8) == 0) {
                if (w_8_faceCombo) {
                    SetInstance(ref w_8_faceCombo);
                }
            } else {
                if (w_5) {
                    SetInstance(ref w_5);
                }
                if ((flag & 1) != 0) {
                    if (w_2.flat) {
                        SetInstance(ref w_2.flat);
                    }
                } else {
                    if (w_2.face) {
                        SetInstance(ref w_2.face);
                    }
                }
                if ((flag & 2) != 0) {
                    if (w_4.flat) {
                        SetInstance(ref w_4.flat);
                    }
                } else {
                    if (w_4.face) {
                        SetInstance(ref w_4.face);
                    }
                }
                if ((flag & 4) != 0) {
                    if (w_6.flat) {
                        SetInstance(ref w_6.flat);
                    }
                } else {
                    if (w_6.face) {
                        SetInstance(ref w_6.face);
                    }
                }
                if ((flag & 8) != 0) {
                    if (w_8.flat) {
                        SetInstance(ref w_8.flat);
                    }
                } else {
                    if (w_8.face) {
                        SetInstance(ref w_8.face);
                    }
                }
                if ((flag & 2) != 0 && (flag & 1) != 0) {
                    if ((flag & 16) != 0) {
                        if (w_1.flat) {
                            SetInstance(ref w_1.flat);
                        }
                    } else {
                        if (w_1.corner) {
                            SetInstance(ref w_1.corner);
                        }
                    }
                } else if ((flag & 2) != 0 && (flag & 1) == 0) {
                    if (w_1.faceL) {
                        SetInstance(ref w_1.faceL);
                    }
                } else if ((flag & 2) == 0 && (flag & 1) != 0) {
                    if (w_1.faceR) {
                        SetInstance(ref w_1.faceR);
                    }
                } else {
                    if (w_1.edge) {
                        SetInstance(ref w_1.edge);
                    }
                }
                if ((flag & 1) != 0 && (flag & 4) != 0) {
                    if ((flag & 32) != 0) {
                        if (w_3.flat) {
                            SetInstance(ref w_3.flat);
                        }
                    } else {
                        if (w_3.corner) {
                            SetInstance(ref w_3.corner);
                        }
                    }
                } else if ((flag & 1) != 0 && (flag & 4) == 0) {
                    if (w_3.faceL) {
                        SetInstance(ref w_3.faceL);
                    }
                } else if ((flag & 1) == 0 && (flag & 4) != 0) {
                    if (w_3.faceR) {
                        SetInstance(ref w_3.faceR);
                    }
                } else {
                    if (w_3.edge) {
                        SetInstance(ref w_3.edge);
                    }
                }
                if ((flag & 8) != 0 && (flag & 2) != 0) {
                    if ((flag & 64) != 0) {
                        if (w_7.flat) {
                            SetInstance(ref w_7.flat);
                        }
                    } else {
                        if (w_7.corner) {
                            SetInstance(ref w_7.corner);
                        }
                    }
                } else if ((flag & 8) != 0 && (flag & 2) == 0) {
                    if (w_7.faceL) {
                        SetInstance(ref w_7.faceL);
                    }
                } else if ((flag & 8) == 0 && (flag & 2) != 0) {
                    if (w_7.faceR) {
                        SetInstance(ref w_7.faceR);
                    }
                } else {
                    if (w_7.edge) {
                        SetInstance(ref w_7.edge);
                    }
                }
                if ((flag & 4) != 0 && (flag & 8) != 0) {
                    if ((flag & 128) != 0) {
                        if (w_9.flat) {
                            SetInstance(ref w_9.flat);
                        }
                    } else {
                        if (w_9.corner) {
                            SetInstance(ref w_9.corner);
                        }
                    }
                } else if ((flag & 4) != 0 && (flag & 8) == 0) {
                    if (w_9.faceL) {
                        SetInstance(ref w_9.faceL);
                    }
                } else if ((flag & 4) == 0 && (flag & 8) != 0) {
                    if (w_9.faceR) {
                        SetInstance(ref w_9.faceR);
                    }
                } else {
                    if (w_9.edge) {
                        SetInstance(ref w_9.edge);
                    }
                }
            }
        }
        for (int i = 0; i < wallTree.Length; i++) {
            if (((flag == 255 && wallTree[i].forInside) || (flag != 255 && !wallTree[i].forInside)) && wallTree[i].prefab && wallTree[i].probability > Random.Range(0, 100)) {
                Instantiate(wallTree[i].prefab, wallsParent.position + wallTree[i].offset, Quaternion.identity, wallsParent);
                break;
            }
        }
        if (wallFadeEnabled) {
            SwitchWallFade swf = GetComponent<SwitchWallFade>();
            if (swf) {
                swf.SetRenderer();
            }
        }
    }

    public void SetEnd(int dir, bool resetTag = true) {
        if (w_end) {
            SetInstance(ref w_end, true, dir == 8 ? 180 : dir == 6 ? -90 : dir == 4 ? 90 : 0, true);
            if (resetTag) {
                gameObject.tag = "Untagged";
            }
            if (wallFadeEnabled) {
                SwitchWallFade swf = GetComponent<SwitchWallFade>();
                if (swf) {
                    swf.enabled = false;
                }
            }
        }
    }
}
