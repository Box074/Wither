using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Modding;
using UnityEngine;

namespace WhiterBoss
{
    public class WitherBossMod : Mod
    {
        public AssetBundle bundle = null;
        public GameObject wither = null;
        public void LoadAB()
        {
#if DEBUG
            bundle = AssetBundle.LoadFromFile(@"G:\WitherBoss\Assets\AssetBundles\wither");
#else
            bundle = AssetBundle.LoadFromStream(typeof(WitherBossMod).Assembly.GetManifestResourceStream("WitherBoss.wither"));
#endif
        }

        public override void Initialize()
        {
            LoadAB();
            wither = bundle.LoadAsset<GameObject>("WitherPrefab");

#if DEBUG
            ModHooks.HeroUpdateHook += ModHooks_HeroUpdateHook;
            //UnityEngine.SceneManagement.SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
#endif
        }

#if DEBUG
        private void SceneManager_activeSceneChanged(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.Scene arg1)
        {
            if (arg1.name.StartsWith("GG_Lost_Kin"))
            {
                foreach (var v in UnityEngine.Object.FindObjectsOfType<HealthManager>(true))
                {
                        var w = UnityEngine.Object.Instantiate(wither);
                        w.transform.position = v.transform.position;
                        w.GetComponent<MoveControl>().target = HeroController.instance.gameObject;
                        w.GetComponent<HealthManager>().OnDeath += WitherBossMod_OnDeath;
                    UnityEngine.Object.Destroy(v.gameObject);
                }
            }
        }

        private void WitherBossMod_OnDeath()
        {
            if (BossSceneController.IsBossScene)
            {
                BossSceneController.Instance.bossesDeadWaitTime = 8;
                BossSceneController.Instance.EndBossScene();
            }
        }

        private void ModHooks_HeroUpdateHook()
        {
            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                var w = UnityEngine.Object.Instantiate(wither);
                w.transform.position = HeroController.instance.transform.position;
                w.GetComponent<MoveControl>().target = HeroController.instance.gameObject;
            }
        }
#endif
    }
}
