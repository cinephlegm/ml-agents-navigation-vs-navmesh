# Repository instructions

This repository is a research artifact, not a generic Unity sample.

## Scientific integrity

- Preserve the original experiment design and clearly label any later reproduction or extension.
- Do not change reported results, hyperparameters or methodological claims without source evidence.
- Distinguish the learned ML-Agent from the conventional Unity NavMesh baseline precisely.
- Do not call the learned system a computer-vision agent unless the reproduced experiment actually uses visual observations. The archived final implementation uses scalar goal information and raycast obstacle observations.
- Keep limitations visible. Do not generalise beyond the controlled experiment.

## Repository hygiene

- Do not copy downloaded literature PDFs into this repository.
- Do not copy the original paid/third-party Unity environment unless redistribution rights are explicitly verified.
- Prefer a simple custom or redistributable environment for reproduction.
- Keep generated Unity folders and ML-Agents training outputs out of Git.
- Store compact derived results and documented configuration, not massive transient logs.

## Validation

Any change to the experiment must document:

1. Unity version;
2. ML-Agents version;
3. observation/action spaces;
4. reward function;
5. training configuration;
6. evaluation protocol;
7. whether the result is original or a later reproduction.
