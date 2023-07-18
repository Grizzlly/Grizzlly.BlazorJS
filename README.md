NOTE: Not tested on MAUI Blazor Hybrid.

NOTE: Breaking changes can appear with every release.

# 🚀 Getting Started

### NuGet package manager
```
Install-Package Grizzlly.BlazorJS
Install-Package Grizzlly.BlazorJS.MSBuild
```

# 🔎 How to use?

### 1. Define NPM Packages

In your `csproj` file define the NPM Packages that you use.

```xml
<PropertyGroup>
  <NPMPackages>@headlessui/vue vue-router@4</NPMPackages
</PropertyGroup>
```

Packages have to be separated by space because they are used directly in
`npm install $(NPMPackages)`.

### 2. Create an `imports.js` file

At the root of your project, create an `imports.js` file (similar to
`_Imports.razor`).

Import and export all of the components that you use. Export names MUST be
unique.

```js
import TestComponent from './src/TestComponent.vue';
import { Switch } from '@headlessui/vue';

export { TestComponent, Switch }
```

This file is injected into the compilation process.

### 3. Use the component with `VueComponent`

This component accepts 3 parameters:
- `ComponentName` (required): The name of the component to render
- `As`: The parent HTML Element tag (eg. `div`). Defaults to `template`.
- `Emits`: Array of emits. You can create an emit by specifying its name and
a callback function that accepts one parameter.

The component accepts any additional parameters which will be passed directly
to the underlying component.

```xml
<VueComponent ComponentName="TestComponent" As="div" Emits="emits" />
```

### 4. (Optional) Create local components for complex functionality

Add all your Vue components to the `src` directory created at the root of your
project. Currently, there is no way of changing this.

Such a component can be:

TestComponent.razor
```html
<script setup lang="ts">
    import { inject } from 'vue';

    const provider = inject('blazorjs') as any;
</script>

<template>
    <div>
        <button @click="provider('click', null)">Hi</button>
    </div>
</template>
```

Use the `inject('blazorjs')` hook to interact with the emits you have provided
in `VueComponent`.

# 💡 Sample

There is a sample project called `BlazorWASM` in the `test` folder.

# ⏰ Future plans

- Ability to provide setup to Vue such as installing plugins. This also
provides the ability to use UI frameworks that require this step such as
[Chakra UI](https://chakra-ui.com/).
- Ability to use React components.
- Ability to render Blazor components inside Vue/React components.
