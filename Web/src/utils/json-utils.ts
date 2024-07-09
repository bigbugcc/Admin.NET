/**
 * 尝试将字符串转对象
 * @param value 要转的字符串
 * @returns {Object|String}
 */
export const StringToObj = (value: any): any => {
	if (value && typeof value == 'string') {
		try {
			const obj = JSON.parse(value);
			if (typeof obj == 'object') {
				return obj;
			} else return value;
		} catch (e) {
			return value;
		}
	} else return value;
};
