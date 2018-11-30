using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VREscape;

[RequireComponent(typeof(Renderer))]
public class Button : MonoBehaviour
{
	private AudioSource _audioSource;
	private Renderer _renderer;
	private HWManager _hwManager;

	public AudioClip ButtonClickSound;
	public AudioClip ButtonReleaseSound;
	public Enums.ButtonEnum ButtonType;

	private bool _isPressed = false;
	
	// Use this for initialization
	void Start ()
	{
		_hwManager = FindObjectOfType<HWManager>();
		_audioSource = FindObjectOfType<AudioSource>();
		_renderer = GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
		if (_hwManager.GetButtonState(ButtonType))
		{
			if (!_isPressed)
			{
				ButtonDown();
			}
			_isPressed = true;
		}
		else
		{
			if (_isPressed)
			{
				ButtonUp();
			}
			_isPressed = false;
		}
	}

	protected virtual void ButtonDown()
	{
		if (_audioSource != null)
		{
			_audioSource.PlayOneShot(ButtonClickSound);
		}
	}

	protected virtual void ButtonUp()
	{
		if (_audioSource != null)
		{
			_audioSource.PlayOneShot(ButtonReleaseSound);
		}
	}
}
