import {markRaw} from 'vue';
const resultComps = {}
let requireComponent = import.meta.glob('./*.vue',{eager: true})
Object.keys(requireComponent).forEach(fileName => {
	//replace(/(\.\/|\.js)/g, '')
	resultComps[fileName.replace(/^\.\/(.*)\.\w+$/, '$1')] = requireComponent[fileName].default
})
export default markRaw(resultComps)

