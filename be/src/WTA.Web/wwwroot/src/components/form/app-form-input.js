import html from '../../utils/index.js';
import { ref, reactive, watch } from 'vue';
import SvgIcon from '../svg-icon.js';

const template = html` <template v-if="schema.type==='boolean'">
    <el-checkbox v-model="model[prop]" />
  </template>
  <template v-else>
    <el-input
      v-model="model[prop]"
      :type="schema.input ?? schema.type === 'string' ? 'text' : schema.type"
      :placeholder="placeholder"
      :show-password="schema.input === 'password'"
      clearable
    >
      <template
        v-if="schema.prefix"
        #prefix
      >
        <svg-icon
          class="el-input__icon"
          :name="schema.prefix"
        />
      </template>
      <template
        v-if="schema.suffix"
        #suffix
      >
        <svg-icon
          class="el-input__icon"
          :name="schema.suffix"
        />
      </template>
    </el-input>
  </template>`;

export default {
  components: { SvgIcon },
  template,
  props: {
    modelValue: {
      type: Object,
      default: null,
    },
    mode: {
      type: String,
      default: 'create',
    },
    prop: {
      type: String,
      default: null,
    },
    schema: {
      type: Object,
      default: null,
    },
  },
  emits: ['update:modelValue', 'before', 'after'],
  setup(props, context) {
    const model = reactive(props.modelValue);
    watch(model, (value) => {
      context.emit('update:modelValue', value);
    });
    const placeholder = props.schema.placeholder ?? props.schema.title;
    return {
      model,
      placeholder,
    };
  },
};
