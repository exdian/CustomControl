# SlideToggleButton �ؼ�˵��

## ����
`SlideToggleButton` ��һ���Զ��� WPF �ؼ������ڱ�ʾ����״̬��������˰�ť����ͻ��黬���Ľ�����ʽ�������û�ͨ��**���**��**����**�������л�����״̬��
![SlideToggleButton ���ʾ��](SlideToggleButton.jpg)

## ���Ĺ���
1.  **״̬�л�**��ͨ������򻬶������ı� `IsChecked` ״̬ (`true`/`false`)��
2.  **����֧��**��ʵ�� `ICommandSource` �ӿڣ�֧�ְ󶨣�
    *   `Command`
    *   `CommandParameter`
    *   `CommandTarget`
3.  **�¼�**��
    *   `Click`����ť����Ч���ʱ������
    *   `Checked`���� `IsChecked` �� `false` ��Ϊ `true` ʱ������
    *   `Unchecked`���� `IsChecked` �� `true` ��Ϊ `false` ʱ������
4.  **��۶���**���ṩ���� `DependencyProperty` ���������Զ����Ӿ���ۺͶ�����
    *   `TrackCornerRadius`
    *   `CheckedTrackColor` (���ؿ���ʱ�Ĺ����ɫ)
    *   `UncheckedTrackColor` (���عر�ʱ�Ĺ����ɫ)
    *   `ThumbColor` (������ɫ)
    *   `Width`
    *   `Height`
    *   *(��۸��ĺ�ؼ���Ϊ��������)*

## ��Ϊ���¼���������
*   **`Click` �¼�**��������갴�µ����ͷŵ�֮��ľ��� **С�� 5 ���߼�����** ʱ���������� **���ڻ���� 5 ���߼�����** �Ĳ�������Ϊ���������ᴥ�� `Click`��
*   **`Checked` / `Unchecked` �¼�**��
    *   �� `IsChecked` ����ֵ������Ӧ�仯ʱ���� (`false` -> `true` ���� `Checked`; `true` -> `false` ���� `Unchecked`)��
    *   �������û�**���**��**����**����������
    *   Ҳ������**ֱ�Ӵ��븳ֵ** `IsChecked` ���Դ�����

## ����д���� (������չ)
��Ϊ����ʱ��������д���·������Զ�����Ϊ��
*   `protected virtual void OnClick();` *(ע�⣺��дʱδ���û���ʵ�� (`base.OnClick()`) ����ֹ `Click` �¼�������)*
*   `protected virtual void OnChecked(RoutedEventArgs e);` *(ע�⣺��дʱδ���û���ʵ�� (`base.OnChecked(e)`) ����ֹ `Checked` �¼�������)*
*   `protected virtual void OnUnchecked(RoutedEventArgs e);` *(ע�⣺��дʱδ���û���ʵ�� (`base.OnUnchecked(e)`) ����ֹ `Unchecked` �¼�������)*
*   `protected virtual void OnToggle();` *(��д�˷���**����**���û���ʵ�� (`base.OnToggle()`))*
*   *(ͬʱ����д�̳��� `Thumb` �������ط���)*

## �Զ��� ControlTemplate �ؼ�Ҫ��
1.  **����״̬����**��
    *   ǿ�ҽ���ʹ�� `VisualStateManager` ����״̬���ɶ�����
    *   `Checked`, `Unchecked`, `Reset` �������Ӿ�״̬ (`VisualState`) **����**������ͬһ�� `VisualStateGroup` �ڡ�
    *   �� `VisualStateGroup` **��Ӧ**���� `Normal` ״̬ (ͨ��������һ��״̬��)��
2.  **���貿��**��
    *   ���Ӿ�����**����**����һ����Ϊ `PART_Thumb` �� `Border` Ԫ�ء�
    *   �� `Border` **����**�������ԣ�`RenderTransform="{TemplateBinding ThumbTransform}"`�����ǿؼ�ʵ�ֻ��黬�����ܵĺ���������
    ```xaml
    <Border x:Name="PART_Thumb" RenderTransform="{TemplateBinding ThumbTransform}" ... />
    ```

## �ߴ罨��
Ϊ�˻������Ӿ��ͽ���Ч����������ѭ���±�����ϵ��
*   `Width > Height` (ʹ��`RenderTransform`���Կ���ʵ�ִ�ֱ��ʽ)
*   `1.5 * Height <= Width <= 2 * Height`

## ���ܳ��ֵ�����
ͨ�������������ı�`VisualState.Storyboard`�е�**����Ŀ��ֵ**����Ȼ�Ѿ�ʵ�ֲ�����δ����bug�����Ǹ��˾��õ�ǰʵ�ֵķ�ʽ�����ȶ��������Խϴ󣬿��ܻ���ֶ�����ɫ��������⣨��Ӱ���Ӿ���**����**���±�����

---