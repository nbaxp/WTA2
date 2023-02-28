import html from '../utils/index.js';
import { reactive,onMounted, ref } from 'vue';

const template = html` <el-card>App List</el-card> `;

export default {
  props: {
    modelValue: {
      type: Object,
      default: null,
    },
  },
  template,
  setup(props) {
    const model = reactive(props.modelValue);
    return {
      model
    }
  },
};
