import html from '../../utils/index.js';
import appFormItem from './app-form-item.js';
import { ref, reactive, watch } from 'vue';

const template = html`
<el-card class="box-card">
    <template #header>
      <div class="card-header">
        <slot name="header">
          <template v-if="model.schema?.title">
            {{ model.schema?.title }}
          </h2>
        </slot>
      </div>
    </template>
    <el-form ref="formRef">
      <app-form-item
        v-model="model"
      />
      <slot name="footer">
        <el-form-item>
          <el-button
            :disabled="disabled"
            type="primary"
            @click="submit"
          >
            {{$t('confirm')}}
          </el-button>
          <el-button @click="reset">{{$t('reset')}}</el-button>
        </el-form-item>
      </slot>
    </el-form>
  </el-card>
`;
export default {
  template,
  components: { appFormItem },
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
