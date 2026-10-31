# ML-Agents Navigation vs NavMesh

A mini research thesis comparing a PPO-trained Unity ML-Agent with a conventional Unity NavMesh navigation baseline in an indoor navigation task.

The project was originally completed for Research Design I and is being republished here as a clean research artifact rather than as a coursework submission dump.

## Research setup

The learned agent is implemented in Unity ML-Agents and receives eight scalar observations:

- goal direction on the X/Z plane;
- normalised distance to the goal;
- five obstacle-distance raycasts spanning -90°, -45°, 0°, 45° and 90°.

It outputs two continuous actions for forward/backward movement and turning.

The conventional baseline uses Unity's `NavMeshAgent` to route toward the same tagged goal.

## Training configuration

The archived experiment configuration uses:

- Unity 2022.3.49f1
- Unity ML-Agents package 2.0.1
- PPO
- batch size: 2048
- buffer size: 20480
- learning rate: 5e-4
- epsilon: 0.2
- lambda: 0.95
- hidden units: 256
- hidden layers: 2
- gamma: 0.99
- maximum training steps: 5,000,000

## Why this repository is being rebuilt

The original university repository also contains milestone ZIPs, draft documents, presentation files, downloaded research papers and a third-party Unity Asset Store environment. Those do not belong in a public source repository.

This clean repository will contain only:

- the experiment implementation;
- training configuration;
- a redistributable/minimal environment;
- methodology and results documentation;
- the final mini-thesis report where redistribution is appropriate;
- demo media and reproducibility instructions.

## Important scope note

This is a small controlled comparison, not evidence that reinforcement learning is generally better than deterministic pathfinding. The public repository will preserve the original experiment and its limitations rather than inflate the claim.

## License

A repository-wide license has not been applied yet because the source code, academic report and third-party assets have different licensing considerations. They will be handled explicitly before public release.
