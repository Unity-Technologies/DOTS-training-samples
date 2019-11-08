using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECSExamples {

public interface ISpawnable {
	void OnSpawned(Spawner spawner);
}

}
