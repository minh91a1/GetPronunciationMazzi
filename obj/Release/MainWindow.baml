   M S B A M L   `   `   `  ���� P  LGetPronunciationMazzi, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=#clr-namespace:GetPronunciationMazziGetPronunciationMazzi  &     GetPronunciationMazzi.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5local#clr-namespace:GetPronunciationMazzi       � Title$  
MainWindow��$��651.801��$��919.492��.��� ����  �� ImageSource$ /sky.jpg�� j� Stretch$ UniformToFill=� �� Opacity$	 0.7Q� � RowDefinitions �� $
��auto ��� $
��auto ��� �� $
��auto �.+��  � ColumnDefinitions �� $
��auto ��� �� $
��auto � �� �� .+�� $��0��.+��� -   $��Input CSV File��$
��Left=�$��Center=�$��
10 10 10 0q�$	��120��� $��1��.+��� -   $��Ignore words��$
��Left=�$��Center=�$��
10 10 10 0q�$	��120��� $��0��$��1��.+��� -   $��csvFileName�� �� Padding$
 10 0q�$��
10 10 10 0q�
�������
�������CSV File Name here ...� $��1��$��1��.+��� -   $��ignoreFileNameTxbBlock��$
 10 0q�$��
10 10 10 0q�
�������
�������!Ignore words File Name here ...� $��0��$��2��.+��� -   $��clearCSVFile��$��
10 10 10 0q�$
��10 0q�.��Remove� $��1��$��2��.+��� -   $��clearIgnoreFile��$��
10 10 10 0q�$
��10 0q�.��Remove� $��1�� �� �� $
��auto �.+���  �� Orientation$ 
Horizontal=�$��0��.+��� -   $��startBtn��$��Start��$
��Left=�$��10q�$��Center=�$	��155��$��60���� -   $��status��$��Center=�$
��Bold"�$��14(�Hello~� $��1��.+��� -	   $��Export To CSV��$��Right=�$��10q�$��Center=�$	��155��$��60��� $��2�� �� .+�(  "System.Windows.Controls.WebBrowser  -
   $	��web��$��Stretch=�$��Stretch=�$��10q�� $��3��.+��� -   $��logBox��$��80��$��10q� �� AcceptsReturn . "	 �� VerticalScrollBarVisibility$	 Visible=�
 J� Opacity$	
 0.5Q�