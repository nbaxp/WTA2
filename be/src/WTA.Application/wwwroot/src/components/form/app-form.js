import html from '../../utils/index.js';
import { ref } from 'vue';

const template = html`<el-form ref="formRef">
  <slot name="header">
    <h2
      v-if="schema?.title"
      class="text-center"
    >
      {{ schema?.title }}
    </h2>
  </slot>

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
</el-form>`;

export default {
  template,
  setup() {
    const formRef = ref(null);
    return {
      formRef,
    };
  },
};
