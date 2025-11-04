# CI/CD Guide for SteelkiltSharp

This document describes the continuous integration and deployment pipelines configured for the SteelkiltSharp project.

## Workflows

### 1. .NET CI (`dotnet.yml`)

**Triggers:** Push and PR to `main`/`master` branches

**Purpose:** Primary build and test workflow

**Steps:**
- Checkout code
- Setup .NET 9.0
- Restore dependencies
- Build in Release configuration
- Run all tests with test result reporting

**Status Badge:**
```markdown
[![.NET CI](https://github.com/tonybierman/steelkilt_sharp/actions/workflows/dotnet.yml/badge.svg)](https://github.com/tonybierman/steelkilt_sharp/actions/workflows/dotnet.yml)
```

---

### 2. Code Coverage (`coverage.yml`)

**Triggers:** Push and PR to `main`/`master` branches

**Purpose:** Generate and report code coverage metrics

**Steps:**
- Build and test with code coverage collection
- Generate coverage report with minimum thresholds (60% warning, 80% target)
- Post coverage summary as PR comment
- Upload coverage to Codecov

**Coverage Thresholds:**
- ⚠️ Warning: Below 60%
- ✅ Target: 80% or higher

**Status Badge:**
```markdown
[![Code Coverage](https://github.com/tonybierman/steelkilt_sharp/actions/workflows/coverage.yml/badge.svg)](https://github.com/tonybierman/steelkilt_sharp/actions/workflows/coverage.yml)
```

---

### 3. Multi-Platform Build (`multi-platform.yml`)

**Triggers:** Push and PR to `main`/`master` branches

**Purpose:** Verify builds work across different operating systems

**Platforms:**
- Ubuntu (Linux)
- Windows
- macOS

**Steps:**
- Build and test on each platform
- Publish build artifacts
- Upload artifacts for download

**Status Badge:**
```markdown
[![Multi-Platform Build](https://github.com/tonybierman/steelkilt_sharp/actions/workflows/multi-platform.yml/badge.svg)](https://github.com/tonybierman/steelkilt_sharp/actions/workflows/multi-platform.yml)
```

---

### 4. Pull Request Validation (`pr-validation.yml`)

**Triggers:** PR opened, synchronized, or reopened

**Purpose:** Comprehensive PR validation

**Steps:**
- Code formatting verification (`dotnet format`)
- Full build
- Run all tests
- Post validation results as PR comment

---

### 5. Release (`release.yml`)

**Triggers:** Git tags matching `v*.*.*` (e.g., `v1.0.0`)

**Purpose:** Automated release creation and NuGet packaging

**Steps:**
- Build and test
- Create NuGet package
- Create GitHub release with package attached
- Include release notes

**To create a release:**
```bash
git tag -a v1.0.0 -m "Release version 1.0.0"
git push origin v1.0.0
```

---

## Dependabot Configuration

**File:** `.github/dependabot.yml`

**Purpose:** Automatic dependency updates

**Updates:**
- NuGet packages (weekly)
- GitHub Actions (weekly)

Dependabot will automatically create PRs to update dependencies when new versions are available.

---

## Local Testing

Before pushing, run these commands locally:

```bash
# Restore dependencies
dotnet restore

# Check code formatting
dotnet format --verify-no-changes

# Build
dotnet build --configuration Release

# Run tests
dotnet test --configuration Release

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

---

## CI/CD Requirements

### Secrets (if needed)
No secrets are currently required. If you want to publish to NuGet.org, add:
- `NUGET_API_KEY` - Your NuGet.org API key

### Branch Protection Rules (Recommended)
Configure on GitHub:
1. Go to Settings → Branches → Branch protection rules
2. Add rule for `main` branch:
   - ✅ Require status checks to pass before merging
   - ✅ Require branches to be up to date before merging
   - Select: `.NET CI`, `Code Coverage`, `Multi-Platform Build`
   - ✅ Require linear history
   - ✅ Include administrators

---

## Troubleshooting

### Build Failures
1. Check the workflow run logs in the Actions tab
2. Verify all tests pass locally
3. Ensure .NET 9.0 SDK is used
4. Check for dependency conflicts

### Coverage Issues
1. Code coverage below 60% will fail the build
2. Add more tests to improve coverage
3. Check `coverage/**/coverage.cobertura.xml` for details

### Platform-Specific Issues
1. Check the Multi-Platform Build workflow
2. Test locally on the failing platform
3. Look for platform-specific dependencies or file paths

---

## Current Test Status

- **Total Tests:** 307
- **Test Projects:** SteelkiltSharp.Tests
- **Test Framework:** xUnit
- **Coverage Tool:** Coverlet

All tests must pass for CI to succeed.

---

## Contributing

When contributing:
1. Create a feature branch
2. Make your changes
3. Run tests locally
4. Push and create a PR
5. Wait for CI checks to pass
6. Request review

CI will automatically:
- Build your code
- Run all tests
- Check code coverage
- Verify multi-platform compatibility
- Post results on your PR
