import html from '../../utils/index.js';
import { ref, reactive, watch } from 'vue';

const template = html` <template
  v-for="(value, key) in model.schema.properties"
  :key="key"
>
  {{model.schema.properties[key].title}}
</template>`;
export default {
  template,
  props: {
    modelValue: {
      type: Object,
      default: null,
    },
  },
  emits: ['update:modelValue', 'before', 'after'],
  setup(props, context) {
    const model = reactive(props.modelValue);
    //context.emit(['update:modelValue', 'before', 'after']);
    watch(model, (value) => {
      context.emit('update:modelValue', value);
    });
    const formRef = ref(null);
    return {
      formRef,
      model,
    };
  },
};
