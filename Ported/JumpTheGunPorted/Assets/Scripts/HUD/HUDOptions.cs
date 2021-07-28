using Unity.Entities;
using UnityEngine;

public class HUDOptions : MonoBehaviour
{
	[Header("Children")]
	public HUDSlider _TerrainWidth;
	public HUDSlider _TerrainLength;
	
	public HUDSlider _MinTerrainHeight;
	public HUDSlider _MaxTerrainHeight;
	public HUDSlider _HeightDamage;
	
	public HUDSlider _TankCount;
	public HUDSlider _TankReloadTime;
	
	private void Start()
	{
		_TerrainWidth.SetBounds(1, 100);
		_TerrainWidth.Value = Config.Instance.HUDData.TerrainWidth;

		_TerrainLength.SetBounds(1, 100);
		_TerrainLength.Value = Config.Instance.HUDData.TerrainLength;

		_MinTerrainHeight.SetBounds(1, 10);
		_MinTerrainHeight.Value = Config.Instance.HUDData.MinTerrainHeight;

		_MaxTerrainHeight.SetBounds(1, 10);
		_MaxTerrainHeight.Value = Config.Instance.HUDData.MaxTerrainHeight;

		_HeightDamage.SetBounds(0, 10);
		_HeightDamage.Value = Config.Instance.HUDData.HeightDamage;

		_TankCount.SetBounds(0, 1000);
		_TankCount.Value = Config.Instance.HUDData.TankCount;

		_TankReloadTime.SetBounds(.1f, 20);
		_TankReloadTime.Value = Config.Instance.HUDData.TankReloadTime;
	}
	
	private void Update()
	{
		UpdateSliders();
	}

	private void UpdateSliders()
	{
		UpdateSlider(out Config.Instance.HUDData.TerrainWidth, _TerrainWidth, "Terrain Width");
		UpdateSlider(out Config.Instance.HUDData.TerrainLength, _TerrainLength, "Terrain Length");
		
		UpdateSlider(out Config.Instance.HUDData.MinTerrainHeight, _MinTerrainHeight, "Min Terrain Height");
		UpdateSlider(out Config.Instance.HUDData.MaxTerrainHeight, _MaxTerrainHeight, "Max Terrain Height");
		UpdateSlider(out Config.Instance.HUDData.HeightDamage, _HeightDamage, "Height Damage");
		
		UpdateSlider(out Config.Instance.HUDData.TankCount, _TankCount, "Tank Count");
		UpdateSlider(out Config.Instance.HUDData.TankReloadTime, _TankReloadTime, "Tank Reload Time");
	}

	private static void UpdateSlider(out int configValue, HUDSlider slider, string label)
	{
		configValue = Mathf.RoundToInt(slider.Value);
		slider.SetText(label + ": " + configValue);
	}

	private static void UpdateSlider(out float configValue, HUDSlider slider, string label)
	{
		configValue = slider.Value;
		slider.SetText(label + ": " + configValue);
	}
	
	public void OnRestart()
	{
		World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity(typeof(Spawner));
		Config.Instance.CommitHUDConfig();
	}
}
