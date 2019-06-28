# Documentation

## General

* [Release 1.0 (Date-TBD)](https://github.com/Informatievlaanderen/registry-documentation/wiki/Release-1.0-(Date-TBD))

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
