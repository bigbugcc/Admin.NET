<template>
	<div>
		<vue-json-pretty :path="'res'" :data="graphData"></vue-json-pretty>
	</div>
</template>

<script setup lang="ts">
import VueJsonPretty from 'vue-json-pretty';
import 'vue-json-pretty/lib/styles.css';

var props = defineProps({
	graphData: Object,
});
</script>
