   M S B A M L   `   `   `  ����P  LGetPronunciationMazzi, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=#clr-namespace:GetPronunciationMazziGetPronunciationMazzi  5   	   &     GetPronunciationMazzi.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   5local#clr-namespace:GetPronunciationMazzi     5   	     � Title$  
MainWindow��5   	   $��651.801��6   $��919.492��6-   .��5	      � ��5
   
   �� 5       �� ImageSource$ /sky.jpg��6    j� Stretch$ UniformToFill=�60    �� Opacity$	 0.7Q�6H   6X    � RowDefinitions 5   
   �� 5      $
��auto �6   �� 5      $
��auto �6   �� 5      �� 5      $
��auto �6   .+�5   
   �  � ColumnDefinitions 5      �� 5      $
��auto �6#   �� 5      �� 5      $
��auto �6#    5      �� 5      �� 5      .+�5      � $��0��6   .+�5      �� -   5        $��Input CSV File��5      $
��Left=�62   $��Center=�6M   $��
10 10 10 0q�6h   $	��120��6|   5    8   5!      � 5"      $��1��6   .+�5#      �� -   5        $��Ignore words��5#      $
��Left=�60   $��Center=�6K   $��
10 10 10 0q�6f   $	��120��6z   5$   7   5%      � 5&      $��0��6   $��1��6    .+�5'      �� -   5        $��csvFileName��5'       �� Padding$
 10 0q�6/   $��
10 10 10 0q�6>   
�������
�������CSV File Name here ...6�   5(      � 5)      $��1��6   $��1��6    .+�5*      �� -   5        $��ignoreFileNameTxbBlock��5*      $
 10 0q�6:   $��
10 10 10 0q�6I   
�������
�������!Ignore words File Name here ...6�   5+      � 5,      $��0��6   $��2��6    .+�5-      �� -   5        $��clearCSVFile��5-      $��
10 10 10 0q�6-   $
��10 0q�6\   .��6k   Remove6s   5.      � 5/      $��1��6   $��2��6    .+�50      �� -   5        $��clearIgnoreFile��50      $��
10 10 10 0q�60   $
��10 0q�6b   .��6q   Remove6y   51      52      � 54   
   $��1��6    55      �� 56      �� 57      $
��auto �6#   .+�5:      ��  �� Orientation$ 
Horizontal=�6   $��0��62   .+�5;      �� -   5        $��startBtn��5;      $��Start��6)   $
��Left=�69   $��10q�6T   $��Center=�6`   $	��155��6{   $��60��6�   5<      �� 5=      -   5        $��status��5=      $��Center=�6*   $
��Bold"�6E   $��14(�6W   Hello~6m   5>      � 5@      $��1��6   .+�5A      �� -	   5        $��Export To CSV��5A      $��Right=�61   $��10q�6M   $��Center=�6Y   $	��155��6t   $��60��6�   5B      5C      5D      � 5F   
   $��2��6    5G      �� 5H      .+�5J      (  "System.Windows.Controls.WebBrowser  -
   5        $	��web��5J      $��Stretch=�6$   $��Stretch=�6B   $��10q�6^   5K      � 5M   
   $��3��6   .+�5N      �� -   5        $��logBox��5N      $��80��6$   $��10q�60    �� AcceptsReturn . "	 �� VerticalScrollBarVisibility$	 Visible=�6Q   
 J� Opacity$	
 0.5Q�6w   5O      5P      5Q      