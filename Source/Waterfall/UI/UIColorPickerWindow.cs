﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Waterfall;


namespace Waterfall.UI
{
  public class UIColorPickerWindow : UIPopupWindow
  {
    int colorFieldSize = 150;
    float sliderWidth = 150f;
    protected string windowTitle = "";

    float rValue = 0f;
    float gValue = 0f;
    float bValue = 0f;
    float aValue = 0f;
    

    string rText = "";
    string gText = "";
    string bText = "";
    string aText = "";

    Texture swatch;

    Texture rTexture;
    Texture gTexture;
    Texture bTexture;
    Texture aTexture;

    Texture rainbow;
    Texture colorField;

    float hueValue = 0f;
    float prevHue = 0f;


    HSBColor currentHSVColor;
    HSBColor prevHSVColor;

    Color currentColor;
    Color savedColor;
    Color prevColor;

    public UIColorPickerWindow(Color colorToEdit, bool show) : base(show)
    {
      
      currentColor = colorToEdit;
      savedColor = colorToEdit;
      prevColor = colorToEdit;

      aValue = currentColor.a * 255f;
      rValue = currentColor.r * 255f;
      gValue = currentColor.g * 255f;
      bValue = currentColor.b * 255f;

      aText = aValue.ToString("F0");
      rText = rValue.ToString("F0");
      gText = gValue.ToString("F0");
      bText = bValue.ToString("F0");
      currentHSVColor = new HSBColor(currentColor);
      prevHSVColor = new HSBColor(currentColor);

      GenerateTextures();
      rainbow = MaterialUtils.GenerateRainbowGradient(120, 20);
      aTexture = MaterialUtils.GenerateGradientTexture(100, 20, Color.black, Color.white);
      colorField = MaterialUtils.GenerateColorField(colorFieldSize, colorFieldSize, currentHSVColor.ToColor());
      
        WindowPosition = new Rect(Screen.width / 2 - 100, Screen.height / 2f, 210, 100);
    }

    protected override void InitUI()
    {
      windowTitle = "Color Picker";
      
      base.InitUI();
    }

    public Color GetColor()
    {
      return currentColor;
    }

    public void ChangeColor(Color colorToEdit)
    {
      currentColor = colorToEdit;
      savedColor = colorToEdit;
      prevColor = colorToEdit;

      aValue = currentColor.a*255f;
      rValue = currentColor.r * 255f;
      gValue = currentColor.g * 255f;
      bValue = currentColor.b * 255f;

      aText = aValue.ToString("F0");
      rText = rValue.ToString("F0");
      gText = gValue.ToString("F0");
      bText = bValue.ToString("F0");

      currentHSVColor = new HSBColor(currentColor);
      prevHSVColor = new HSBColor(currentColor);

      GenerateTextures();
      rainbow = MaterialUtils.GenerateRainbowGradient(120, 20);
      aTexture = MaterialUtils.GenerateGradientTexture(100, 20, Color.black, Color.white);
      colorField = MaterialUtils.GenerateColorField(colorFieldSize, colorFieldSize, currentHSVColor.ToColor());

      if (!showWindow)
        WindowPosition = new Rect(Screen.width / 2 - 100, Screen.height / 2f, 210, 100);

      GUI.BringWindowToFront(windowID);
    }

    protected override void DrawWindow(int windowId)
    {
      // Draw the header/tab controls
      DrawTitle();
      DrawColorField();
      DrawSliders();
      GUI.DragWindow();

    }

    protected void DrawTitle()
    {
      GUILayout.BeginHorizontal();
      GUILayout.Label(windowTitle, GUIResources.GetStyle("window_header"), GUILayout.MaxHeight(26f), GUILayout.MinHeight(26f));

      GUILayout.FlexibleSpace();

      Rect buttonRect = GUILayoutUtility.GetRect(22f, 22f);
      GUI.color = resources.GetColor("cancel_color");
      if (GUI.Button(buttonRect, "", GUIResources.GetStyle("button_cancel")))
      {
        ToggleWindow();
      }

      GUI.DrawTextureWithTexCoords(buttonRect, GUIResources.GetIcon("cancel").iconAtlas, GUIResources.GetIcon("cancel").iconRect);
      GUI.color = Color.white;
      GUILayout.EndHorizontal();
    }
    protected void DrawColorField()
    {
      GUILayout.BeginVertical();
      GUILayout.Space(4);
      Rect tRect = GUILayoutUtility.GetRect(120, 20, GUILayout.Width(sliderWidth));
      GUI.DrawTexture(tRect, swatch);
      GUILayout.Space(4);
      tRect = GUILayoutUtility.GetRect(150, 20, GUILayout.Width(sliderWidth));
      GUI.DrawTexture(tRect,rainbow);
      hueValue = GUILayout.HorizontalSlider(hueValue, 0f, 1f, GUILayout.Width(sliderWidth));
      currentHSVColor = new HSBColor(hueValue, currentHSVColor.s, currentHSVColor.b);
      tRect = GUILayoutUtility.GetRect(colorFieldSize, colorFieldSize, GUILayout.Width(colorFieldSize));
      GUI.DrawTexture(tRect, colorField);

      var eventType = Event.current.type;
      if (eventType == EventType.Repaint)
        if (Input.GetMouseButtonDown(0))
        {
          Vector2 screenPoint = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
          Rect screenRect = GUIUtility.GUIToScreenRect(tRect);

        
          if (tRect.Contains(Event.current.mousePosition))
          {
            int x = (int)Mathf.Clamp(Event.current.mousePosition.x-tRect.xMin,0,sliderWidth);
            int y = colorFieldSize - (int)Mathf.Clamp(Event.current.mousePosition.y - tRect.yMin, 0, sliderWidth);
            Color sampled = ((Texture2D)colorField).GetPixel(x, y);
            Utils.Log($"Color picked at {x}, {y} is {sampled}", LogType.UI);
            currentHSVColor = new HSBColor(sampled);
            currentColor = new Color(sampled.r, sampled.g, sampled.b, currentColor.a);
            aValue = currentColor.a * 255f;
            rValue = currentColor.r * 255f;
            gValue = currentColor.g * 255f;
            bValue = currentColor.b * 255f;

            aText = aValue.ToString("F0");
            rText = rValue.ToString("F0");
            gText = gValue.ToString("F0");
            bText = bValue.ToString("F0");
          }
        }

      GUILayout.EndVertical();
      GUILayout.Space(4);
      


      

      if (!currentHSVColor.ToColor().Equals(prevHSVColor.ToColor()))
      {
        colorField = MaterialUtils.GenerateColorField(colorFieldSize, colorFieldSize, currentHSVColor.ToColor());
        prevHSVColor = currentHSVColor;
        
      }
      
    }
    protected void DrawSliders()
    {
      float sliderValue;
      string fieldText;
      GUILayout.BeginHorizontal();
      GUILayout.Label("R");
      GUILayout.BeginVertical();
      Rect tRect = GUILayoutUtility.GetRect(sliderWidth, 20, GUILayout.Width(sliderWidth));
      GUI.DrawTexture(tRect, rTexture);
      sliderValue = GUILayout.HorizontalSlider(rValue, 0f, 255f, GUILayout.Width(sliderWidth));
      GUILayout.EndVertical();

      if (sliderValue != rValue)
      {
        rValue = sliderValue;
        rText = rValue.ToString("F0");
      }

      fieldText = GUILayout.TextArea(rText);

      if (fieldText != rText)
      {
        if (float.TryParse(fieldText, out rValue))
        {
           rText = fieldText;
        }
      }
      
      GUILayout.EndHorizontal();


      GUILayout.BeginHorizontal();
      GUILayout.Label("G");
      GUILayout.BeginVertical();
      tRect = GUILayoutUtility.GetRect(sliderWidth, 20, GUILayout.Width(sliderWidth));
      GUI.DrawTexture(tRect, gTexture);
      sliderValue = GUILayout.HorizontalSlider(gValue, 0f, 255f, GUILayout.Width(sliderWidth));
      GUILayout.EndVertical();

      if (sliderValue != gValue)
      {
        gValue = sliderValue;
        gText = gValue.ToString("F0");
      }

      fieldText = GUILayout.TextArea(gText);

      if (fieldText != gText)
      {
        if (float.TryParse(fieldText, out gValue))
        {
          gText = fieldText;
        }
      }
      
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal();
      GUILayout.Label("B");
      GUILayout.BeginVertical();
      tRect = GUILayoutUtility.GetRect(sliderWidth, 20, GUILayout.Width(sliderWidth));
      GUI.DrawTexture(tRect, bTexture);
      sliderValue = GUILayout.HorizontalSlider(bValue, 0f, 255f, GUILayout.Width(sliderWidth));
      GUILayout.EndVertical();

      if (sliderValue != bValue)
      {
        bValue = sliderValue;
        bText = bValue.ToString("F0");
      }

      fieldText = GUILayout.TextArea(bText);
      if (fieldText != bText)
      {
        if (float.TryParse(fieldText, out bValue))
        {
          bText = fieldText;
        }
      }
      
      GUILayout.EndHorizontal();
      GUILayout.BeginHorizontal();
      GUILayout.Label("A");
      GUILayout.BeginVertical();
      tRect = GUILayoutUtility.GetRect(sliderWidth, 20, GUILayout.Width(sliderWidth));
      GUI.DrawTexture(tRect, aTexture);
      sliderValue = GUILayout.HorizontalSlider(aValue, 0f, 255f, GUILayout.Width(sliderWidth));
      GUILayout.EndVertical();

      if (sliderValue != aValue)
      {
        aValue = sliderValue;
        aText = aValue.ToString();
      }

      fieldText = GUILayout.TextArea(aText);
      if (fieldText != aText)
      {
        if (float.TryParse(fieldText, out aValue))
        {
          aText = fieldText;
        }
      }
      
      GUILayout.EndHorizontal();

      currentColor = new Color(rValue / 255f, gValue / 255f, bValue / 255f, aValue / 255f);
      
    }

    public void Update()
    {
      if (!currentColor.Equals(prevColor))
      {
        GenerateTextures();

        currentHSVColor = new HSBColor(currentColor);
        hueValue = currentHSVColor.h;
        colorField = MaterialUtils.GenerateColorField(colorFieldSize, colorFieldSize, currentHSVColor.ToColor());
        prevColor = currentColor;
      }
    }

    public void GenerateTextures()
    {
      rTexture = MaterialUtils.GenerateGradientTexture(100,20, new Color(0.0f, currentColor.g, currentColor.b, 1.0f), new Color(1.0f, currentColor.g, currentColor.b, 1.0f));
      gTexture = MaterialUtils.GenerateGradientTexture(100, 20, new Color(currentColor.r, 0f, currentColor.b, 1.0f), new Color(currentColor.r, 1f, currentColor.b, 1.0f));
      bTexture = MaterialUtils.GenerateGradientTexture(100, 20, new Color(currentColor.r, currentColor.g, 0f, 1.0f), new Color(currentColor.r, currentColor.g, 1f, 1.0f));
      
      swatch = MaterialUtils.GenerateColorTexture(60, 20, currentColor);
    }
  }
}

