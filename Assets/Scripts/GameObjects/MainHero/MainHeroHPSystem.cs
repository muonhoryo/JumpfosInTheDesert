using Registry;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamingObjects
{
    public sealed class MainHeroHPSystem : HitPointSystem, ISingltone<MainHeroHPSystem>
    {
        [SerializeField]
        private MainHero MainHeroScript;
        public override GameCharacter Owner => MainHeroScript;
        MainHeroHPSystem ISingltone<MainHeroHPSystem>.Singltone { get => (MainHeroHPSystem)MainHeroScript.HitPointSystem;set { } }
        private void Awake()
        {
            if((MainHeroHPSystem)MainHeroScript.HitPointSystem!= this)
            {
                Destroy(this);
            }
        }
    }
}
