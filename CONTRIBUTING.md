# Contributing

Thanks for taking the time to contribute! 🎉

Contributions of all kinds are welcome: bug reports, feature requests, documentation improvements, and code changes.

Please read this document before contributing to keep things smooth and consistent.

---

## Getting Started

1. Fork the repository
2. Create a feature or fix branch from `main`
3. Make your changes
4. Open a Pull Request

Branch naming convention:

```
main
├─ feature/my-feature
├─ bugfix/fix-something
```

Keep branches focused and short-lived.

Common branch types:

- `feat` – new features
- `fix` – bug fixes
- `docs` – documentation only changes
- `refactor` – code changes without behavior changes
- `chore` – tooling, CI, or dependency updates

---

## Development Guidelines

### Code Style

- Follow existing code conventions
- Prefer clarity over cleverness
- Avoid unnecessary abstractions
- Public APIs should be well-documented

### Testing

- Add or update tests where applicable
- Ensure all existing tests pass before submitting a PR

### Breaking Changes

If your change introduces a breaking API change:

- Call it out clearly in the PR description
- Update documentation where needed
- Expect the change to be released as a **major version**

---

## Versioning & Releases

This project follows **Semantic Versioning (SemVer)**:

```
MAJOR.MINOR.PATCH
```

- **MAJOR** – breaking changes
- **MINOR** – new features (backward compatible)
- **PATCH** – bug fixes

Versions are managed by maintainers during the release process.

---

## Pull Requests

When opening a Pull Request:

- Clearly describe **what** changed and **why**
- Link related issues where applicable
- Mention breaking changes explicitly
- Keep PRs focused (one concern per PR)

Before requesting review, please ensure:

- [ ] The code builds successfully
- [ ] All tests pass
- [ ] Documentation is updated if needed
- [ ] Commit messages follow the convention

---

## Reporting Bugs

When reporting a bug, please include:

- A clear description of the issue
- Steps to reproduce
- Expected vs actual behavior
- Environment details (OS, runtime, project version)

---

## Feature Requests

Feature ideas are welcome!

Please open an issue describing:

- The problem you're trying to solve
- A proposed solution (if any)
- Alternatives you've considered

---

