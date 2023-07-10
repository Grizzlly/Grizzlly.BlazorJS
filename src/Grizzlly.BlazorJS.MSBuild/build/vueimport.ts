import { createApp, type Component, type ComponentPublicInstance } from "vue";

export async function attachVueComponent(
    elementid: string,
    modulename: string,
    attributeKeys: string[],
    attributeValues: object[],
    provider: (s: string, o: object | null) => void
) {
    const modules = (await import("./components")) as any;

    const module = modules[modulename] as Component;

    const el = document.querySelectorAll("[_bl_" + elementid + "]")[0];

    let attrs: any = {};
    for (let i = 0; i < attributeKeys.length; i++) {
        attrs[attributeKeys[i]] = attributeValues[i];
    }

    if (el) {
        var app = createApp(module, attrs);
        app.provide("blazorjs", provider);
        var mounted = app.mount(el) as ComponentPublicInstance;

        return mounted;
    }

    return null;
}
