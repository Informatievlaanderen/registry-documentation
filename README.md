# Documentation

## General

* [Release Notes](https://github.com/Informatievlaanderen/registry-documentation/wiki/Release-Notes)

## Generating

To generate the documentation site, you can use the following commands:

```bash
pip install mkdocs
pip install mkdocs-material

mkdocs new registry-documentation
cd registry-documentation

mkdocs serve
mkdocs build --clean
mkdocs gh-deploy
```
