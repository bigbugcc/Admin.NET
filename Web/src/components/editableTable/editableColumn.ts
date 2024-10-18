export interface EditableColumn {
	id: number;
	label: string;
	prop: string;
	type?: 'input' | 'select';
	options?: EditableColumnOption[];
	editable?: boolean;
	width?: number;
}
export interface EditableColumnOption {
	label: string;
	value: any;
}
