<script setup lang="ts">
    import { inject } from 'vue';

    const provider = inject('blazorjs') as any;
</script>

<template>
    <div>
        <button @click="provider('click', null)">Hi</button>
    </div>
</template>