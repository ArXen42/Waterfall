﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waterfall
{

  /// <summary>
  /// A controller that pulls from throttle settings
  /// </summary>
  [System.Serializable]
  public class RCSController : WaterfallController
  {
    public float currentThrottle = 1;
    ModuleRCSFX rcsController;

    public override void SetupController(ConfigNode node)
    {
      name = "rcs";
      linkedTo = "rcs";
      node.TryGetValue("name", ref name);
    }
    public override void Initialize(ModuleWaterfallFX host)
    {
      base.Initialize(host);

      rcsController = host.GetComponents<ModuleRCSFX>().ToList().First();
      if (rcsController == null)
        rcsController = host.GetComponent<ModuleRCSFX>();

      if (rcsController == null)
        Utils.LogError("[RCSController] Could not find ModuleRCSFX on Initialize");

    }
    public override List<float> Get()
    {
      if (rcsController == null)
      {
        Utils.LogWarning("[RCSController] RCS controller not assigned");
        return new List<float>() { 0f };
      }
      if (overridden)
      {
        List<float> overrideValues = new List<float>();
        for (int i=0; i< rcsController.thrusterTransforms.Count; i++)
        {
          overrideValues.Add(overrideValue);
        }
        return overrideValues;
      }

    
      return rcsController.thrustForces.ToList();
      
    }
  }

}
