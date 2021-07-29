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

	private ConfigData _Data;
	
	private void Start()
	{
		_Data = Config.Instance.CreateConfigData();
		
		_TerrainWidth.SetBounds(1, 100);
		_TerrainWidth.Value = _Data.TerrainWidth;

		_TerrainLength.SetBounds(1, 100);
		_TerrainLength.Value = _Data.TerrainLength;

		_MinTerrainHeight.SetBounds(1, 10);
		_MinTerrainHeight.Value = _Data.MinTerrainHeight;

		_MaxTerrainHeight.SetBounds(1, 10);
		_MaxTerrainHeight.Value = _Data.MaxTerrainHeight;

		_HeightDamage.SetBounds(0, 10);
		_HeightDamage.Value = _Data.HeightDamage;

		_TankCount.SetBounds(0, 1000);
		_TankCount.Value = _Data.TankCount;

		_TankReloadTime.SetBounds(.1f, 20);
		_TankReloadTime.Value = _Data.TankReloadTime;
	}
	
	private void Update()
	{
		UpdateSliders();
	}

	private void UpdateSliders()
	{
		UpdateSlider(out _Data.TerrainWidth, _TerrainWidth, "Terrain Width");
		UpdateSlider(out _Data.TerrainLength, _TerrainLength, "Terrain Length");
		
		UpdateSlider(out _Data.MinTerrainHeight, _MinTerrainHeight, "Min Terrain Height");
		UpdateSlider(out _Data.MaxTerrainHeight, _MaxTerrainHeight, "Max Terrain Height");
		UpdateSlider(out _Data.HeightDamage, _HeightDamage, "Height Damage");
		
		UpdateSlider(out _Data.TankCount, _TankCount, "Tank Count");
		UpdateSlider(out _Data.TankReloadTime, _TankReloadTime, "Tank Reload Time");
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
		Config.Instance.Data = _Data;
	}
}
